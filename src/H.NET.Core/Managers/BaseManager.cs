using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core.CustomEventArgs;
using H.NET.Core.Recorders;

namespace H.NET.Core.Managers
{
    public class BaseManager : ParentRecorder
    {
        #region Properties

        public new IRecorder Recorder {
            get => base.Recorder;
            set {
                if (value == null && base.Recorder != null)
                {
                    base.Recorder.Started -= Recorder_OnStarted;
                    base.Recorder.Stopped -= Recorder_OnStopped;
                }

                base.Recorder = value;

                if (base.Recorder != null)
                {
                    base.Recorder.Started += Recorder_OnStarted;
                    base.Recorder.Stopped += Recorder_OnStopped;
                }
            }
        }

        public IConverter Converter { get; set; }
        public List<IConverter> AlternativeConverters { get; set; } = new List<IConverter>();

        public string Text { get; private set; }

        #endregion

        #region Events

        public event EventHandler<string> NewText;
        private void OnNewText() => NewText?.Invoke(this, Text);

        #endregion

        #region Constructors

        #endregion

        #region Public methods

        public void ProcessText(string text)
        {
            Text = text;
            OnNewText();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task ProcessSpeechAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            if (Converter == null)
            {
                Log("Converter is not found");
                return;
            }

            try
            {
                if (Converter.IsStreamingRecognitionSupported)
                {
                    return;
                }

                var text = await Converter.ConvertAsync(bytes, cancellationToken);
                if (!AlternativeConverters.Any())
                {
                    //Log("No alternative converters");
                    ProcessText(text);
                    return;
                }

                var alternativeTexts = AlternativeConverters.Select(async i => await i.ConvertAsync(bytes, cancellationToken)).ToList();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    //Log("Text is not empty. No alternative converters is uses");
                    ProcessText(text);
                    return;
                }

                //Log("Loop");
                while (alternativeTexts.Any())
                {
                    //Log("WhenAny");
                    var alternativeTextTask = await Task.WhenAny(alternativeTexts);
                    var alternativeText = await alternativeTextTask;
                    if (string.IsNullOrWhiteSpace(alternativeText))
                    {
                        //Log("string.IsNullOrWhiteSpace");
                        alternativeTexts.Remove(alternativeTextTask);
                        continue;
                    }

                    //Log("ProcessText");
                    ProcessText(text);
                    return;
                }

                //Log("ProcessOriginalText");
                ProcessText(text);
            }
            catch (Exception exception)
            {
                Log($"{exception}");
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (IsStarted)
            {
                return;
            }

            Text = null;
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!IsStarted)
            {
                return;
            }

            if (Recorder == null)
            {
                Log("Recorder is not found");
                return;
            }

            await Recorder.StopAsync(cancellationToken);
        }

        public async Task ChangeAsync(CancellationToken cancellationToken = default)
        {
            if (!IsStarted)
            {
                await StartAsync(cancellationToken);
            }
            else
            {
                await StopAsync(cancellationToken);
            }
        }

        #endregion

        #region Event handlers

        private async void Recorder_OnStarted(object sender, EventArgs args)
        {
            if (Recorder == null)
            {
                Log("Recorder is not found");
                return;
            }

            if (Converter == null ||
                !Converter.IsStreamingRecognitionSupported)
            {
                return;
            }

            using var recognition = await Converter.StartStreamingRecognitionAsync();
            recognition.AfterPartialResults += (o, e) => ProcessText($"deskband preview {e.Text}");
            recognition.AfterFinalResults += (o, e) =>
            {
                ProcessText("deskband clear-preview");
                ProcessText(e.Text);
            };

            if (Recorder.RawData != null)
            {
                await recognition.WriteAsync(Recorder.RawData.ToArray());
            }

            // ReSharper disable once AccessToDisposedClosure
            async void RecorderOnRawDataReceived(object o, RecorderEventArgs e)
            {
                await recognition.WriteAsync(e.RawData.ToArray());
            }

            try
            {
                Recorder.RawDataReceived += RecorderOnRawDataReceived;

                while (Recorder.IsStarted)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1));
                }

                await recognition.StopAsync();
            }
            finally
            {
                Recorder.RawDataReceived -= RecorderOnRawDataReceived;
            }
        }

        private async void Recorder_OnStopped(object sender, RecorderEventArgs args)
        {
            IsStarted = false;

            if (Recorder == null)
            {
                Log("Recorder is not found");
                return;
            }

            RawData = Recorder.RawData;
            WavData = Recorder.WavData;
            OnStopped(new RecorderEventArgs
            {
                RawData = RawData,
                WavData = WavData,
            });

            await ProcessSpeechAsync(WavData.ToArray());
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Recorder?.Dispose();
            Recorder = null;

            Converter?.Dispose();
            Converter = null;

            base.Dispose();
        }

        #endregion
    }
}
