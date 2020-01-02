using System;
using System.Collections.Generic;
using H.NET.Core.CustomEventArgs;

namespace H.NET.Core.Recorders
{
    public class Recorder : Module, IRecorder
    {
        #region Properties

        public bool IsStarted { get; protected set; }
        public IReadOnlyCollection<byte> RawData { get; protected set; }
        public IReadOnlyCollection<byte> WavData { get; protected set; }

        #endregion

        #region Events

        public event EventHandler Started;
        protected void OnStarted() => Started?.Invoke(this, EventArgs.Empty);


        public event EventHandler<RecorderEventArgs> Stopped;
        protected void OnStopped(RecorderEventArgs args) => Stopped?.Invoke(this, args);

        /// <summary>
        /// When new partial raw data
        /// </summary>
        public event EventHandler<RecorderEventArgs> NewRawData;
        protected void OnNewRawData(IReadOnlyCollection<byte> value) => NewRawData?.Invoke(this, new RecorderEventArgs
        {
            RawData = value,
        });

        #endregion

        #region Public methods

        public virtual void Start()
        {
            if (IsStarted)
            {
                return;
            }

            IsStarted = true;
            RawData = null;
            WavData = null;

            OnStarted();
        }

        public virtual void Stop()
        {
            if (!IsStarted)
            {
                return;
            }

            IsStarted = false;
            OnStopped(new RecorderEventArgs
            {
                RawData = RawData,
                WavData = WavData,
            });
        }

        #endregion
    }
}
