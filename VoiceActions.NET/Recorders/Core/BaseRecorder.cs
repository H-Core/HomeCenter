using System;

namespace VoiceActions.NET.Recorders.Core
{
    public class BaseRecorder : IRecorder
    {
        #region Properties

        public bool IsStarted { get; protected set; }
        public byte[] Data { get; protected set; }

        #endregion

        #region Events

        public event EventHandler<VoiceActionsEventArgs> Started;
        protected void OnStarted(VoiceActionsEventArgs args) => Started?.Invoke(this, args);

        public event EventHandler<VoiceActionsEventArgs> Stopped;
        protected void OnStopped(VoiceActionsEventArgs args) => Stopped?.Invoke(this, args);

        private VoiceActionsEventArgs CreateArgs() => 
            new VoiceActionsEventArgs { Recorder = this, Data = Data };

        #endregion

        #region Public methods

        public virtual void Start()
        {
            IsStarted = true;
            Data = null;
            OnStarted(CreateArgs());
        }

        public virtual void Stop()
        {
            IsStarted = false;
            OnStopped(CreateArgs());
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
