using System;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Recorders;
using VoiceActions.NET.Recorders.Core;

namespace VoiceActions.NET
{
    public class VoiceManager : BaseRecorder
    {
        #region Fields

        private IRecorder _recorder;
        private IConverter _converter;

        #endregion

        #region Properties

        public IRecorder Recorder {
            get => _recorder;
            set {
                _recorder = value ?? throw new Exception("Recorder is null");
                _recorder.Stopped += OnStoppedRecorder;
            }
        }

        public IConverter Converter {
            get => _converter;
            set => _converter = value ?? throw new Exception("Converter is null");
        }

        public string Text { get; private set; }

        #endregion

        #region Events

        public event EventHandler<VoiceActionsEventArgs> NewText;
        private void OnNewText() => NewText?.Invoke(this, new VoiceActionsEventArgs
        {
            Recorder = Recorder,
            Converter = Converter,
            Data = Data,
            Text = Text
        });

        #endregion

        #region Constructors

        public VoiceManager()
        {
        }

        public VoiceManager(IRecorder recorder, IConverter converter) : this()
        {
            Recorder = recorder;
            Converter = converter;
        }

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

        public new void Start()
        {
            var recorder = Recorder ?? throw new Exception("Recorder is null");

            recorder.Start();
            base.Start();
        }

        public void Start(bool disableAutoStopIfExists)
        {
            var recorder = Recorder ?? throw new Exception("Recorder is null");
            if (disableAutoStopIfExists && recorder is IAutoStopRecorder autoStopRecorder)
            {
                autoStopRecorder.AutoStopEnabled = false;
            }

            Start();
        }

        public new void Stop()
        {
            var recorder = Recorder ?? throw new Exception("Recorder is null");
            if (recorder is IAutoStopRecorder autoStopRecorder)
            {
                autoStopRecorder.AutoStopEnabled = true;
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

        #endregion

        #region Event handlers

        private void OnStoppedRecorder(object sender, VoiceActionsEventArgs args)
        {
            base.Stop();
            var recorder = Recorder ?? throw new Exception("Recorder is null");

            Data = recorder.Data;
            ProcessSpeech(Data);
        }

        #endregion

        #region IDisposable

        public new void Dispose()
        {
            _recorder?.Dispose();
            _recorder = null;

            _converter?.Dispose();
            _converter = null;
        }

        #endregion
    }
}
