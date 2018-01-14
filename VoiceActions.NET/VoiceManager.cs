using System;
using System.Collections.Generic;
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

        private Dictionary<string, Action> ActionDictionary { get; } = new Dictionary<string, Action>();
        public string Text { get; private set; }

        #endregion

        #region Events

        public event EventHandler<VoiceActionsEventArgs> NewText;
        private void OnNewText(bool isHandled) => NewText?.Invoke(this, new VoiceActionsEventArgs
        {
            Recorder = Recorder,
            Converter = Converter,
            IsHandled = isHandled,
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

        public void SetActionHandler(string text, Action action)
        {
            ActionDictionary[text] = action;
        }

        public void ProcessText(string text)
        {
            Text = text;
            var isHandled = ActionDictionary.ContainsKey(text) && ActionDictionary[text] != null;
            OnNewText(isHandled);

            if (isHandled)
            {
                ActionDictionary[text]?.Invoke();
            }
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
            Recorder?.Dispose();
            Recorder = null;

            Converter?.Dispose();
            Converter = null;
        }

        #endregion
    }
}
