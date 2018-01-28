using System;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Recorders;
using VoiceActions.NET.Recorders.Core;

namespace VoiceActions.NET
{
    public class VoiceManager : BaseRecorder, IRecorder
    {
        #region Fields

        private IRecorder _recorder;

        #endregion

        #region Properties

        public IRecorder Recorder {
            get => _recorder;
            set {
                if (value == null && _recorder != null)
                {
                    _recorder.Stopped -= OnStoppedRecorder;
                }

                _recorder = value;

                if (value != null)
                {
                    _recorder.Stopped += OnStoppedRecorder;
                }
            }
        }

        public IConverter Converter { get; set; }

        public string Text { get; private set; }

        public new bool AutoStopEnabled
        {
            get => Recorder?.AutoStopEnabled ?? false;
            set
            {
                if (Recorder == null)
                {
                    return;
                }

                Recorder.AutoStopEnabled = value;
            }
        }

        public new int Interval => Recorder?.Interval ?? 0;

        #endregion

        #region Events

        public event EventHandler<VoiceActionsEventArgs> NewText;
        private void OnNewText() => NewText?.Invoke(this, CreateArgs());
        
        protected VoiceActionsEventArgs CreateArgs() => new VoiceActionsEventArgs
        {
            Recorder = Recorder,
            Converter = Converter,
            Data = Data,
            Text = Text
        };

        #endregion

        #region Constructors

        #endregion

        #region Public methods

        public void ProcessText(string text)
        {
            Text = text;
            OnNewText();
        }

        public async void ProcessSpeech(byte[] bytes)
        {
            var converter = Converter ?? throw new Exception("Converter is null");

            ProcessText(await converter.Convert(bytes));
        }

        public override void Start()
        {
            var recorder = Recorder ?? throw new Exception("Recorder is null");

            recorder.Start();
            IsStarted = true;
            Text = null;
            Data = null;
            OnStarted(CreateArgs());
        }

        public void StartWithoutAutostop()
        {
            var recorder = Recorder ?? throw new Exception("Recorder is null");

            if (recorder.Interval > 0)
            {
                recorder.AutoStopEnabled = false;
            }
            Start();
        }

        public override void Stop()
        {
            var recorder = Recorder ?? throw new Exception("Recorder is null");

            if (recorder.Interval > 0)
            {
                recorder.AutoStopEnabled = true;
            }
            recorder.Stop();
        }

        public void Change()
        {
            if (!IsStarted)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        public void ChangeWithoutAutostop()
        {
            if (!IsStarted)
            {
                StartWithoutAutostop();
            }
            else
            {
                Stop();
            }
        }

        #endregion

        #region Event handlers

        private void OnStoppedRecorder(object sender, VoiceActionsEventArgs args)
        {
            IsStarted = false;

            var recorder = Recorder ?? throw new Exception("Recorder is null");

            Data = recorder.Data;
            OnStopped(CreateArgs());

            ProcessSpeech(Data);
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            _recorder?.Dispose();
            _recorder = null;

            Converter?.Dispose();
            Converter = null;
        }

        #endregion
    }
}
