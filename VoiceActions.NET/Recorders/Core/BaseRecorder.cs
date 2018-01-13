using System;
using VoiceActions.NET.Converters;

namespace VoiceActions.NET.Recorders.Core
{
    public class BaseRecorder : IRecorder
    {
        #region Properties

        public bool IsStarted { get; private set; }
        public byte[] Data { get; protected set; }
        public string Text { get; protected set; }

        public IConverter Converter { get; set; }

        #endregion

        #region Events

        public event EventHandler<RecorderEventArgs> Started;
        private void OnStarted() => Started?.Invoke(this, new RecorderEventArgs { SpeechRecorder = this });

        public event EventHandler<RecorderEventArgs> Stopped;
        private void OnStopped() => Stopped?.Invoke(this, new RecorderEventArgs { SpeechRecorder = this, Data = Data, Text = Text });

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
