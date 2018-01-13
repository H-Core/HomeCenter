using System;

namespace VoiceActions.NET.Recorders.Core
{
    public class BaseRecorder : IRecorder
    {
        #region Properties

        public bool IsStarted { get; private set; }
        public byte[] Data { get; protected set; }

        #endregion

        #region Events

        public event EventHandler<VoiceActionsEventArgs> Started;
        private void OnStarted() => Started?.Invoke(this, new VoiceActionsEventArgs { Recorder = this });

        public event EventHandler<VoiceActionsEventArgs> Stopped;
        private void OnStopped() => Stopped?.Invoke(this, new VoiceActionsEventArgs { Recorder = this, Data = Data });

        #endregion

        #region Public methods

        public void Start()
        {
            IsStarted = true;
            OnStarted();
        }

        public void Stop()
        {
            IsStarted = false;
            OnStopped();
        }

        #endregion
    }
}
