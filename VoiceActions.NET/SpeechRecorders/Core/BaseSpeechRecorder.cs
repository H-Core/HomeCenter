using System;
using VoiceActions.NET.SpeechToTextConverters;

namespace VoiceActions.NET.SpeechRecorders.Core
{
    public class BaseSpeechRecorder : ISpeechRecorder
    {
        #region Properties

        public bool IsStarted { get; private set; }
        public byte[] Data { get; protected set; }
        public string Text { get; protected set; }

        public ISpeechToTextConverter Converter { get; set; }

        #endregion

        #region Events

        public event EventHandler<SpeechEventArgs> Started;
        private void OnStarted() => Started?.Invoke(this, new SpeechEventArgs { SpeechRecorder = this });

        public event EventHandler<SpeechEventArgs> Stopped;
        private void OnStopped() => Stopped?.Invoke(this, new SpeechEventArgs { SpeechRecorder = this, Data = Data, Text = Text });

        #endregion

        #region Public methods

        public void Start()
        {
            IsStarted = true;
            OnStarted();
        }

        public async void Stop()
        {
            IsStarted = false;

            if (Converter != null)
            {
                Text = await Converter.Convert(Data);
            }

            OnStopped();
        }

        #endregion
    }
}
