using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core.CustomEventArgs;

namespace H.NET.Core.Recorders
{
    public class Recorder : Module, IRecorder
    {
        #region Properties

        public bool IsInitialized { get; protected set; }
        public bool IsStarted { get; protected set; }
        public IReadOnlyCollection<byte> RawData { get; protected set; }
        public IReadOnlyCollection<byte> WavData { get; protected set; }
        public IReadOnlyCollection<byte> WavHeader { get; protected set; }

        #endregion

        #region Events

        public event EventHandler Started;

        public event EventHandler<RecorderEventArgs> Stopped;

        /// <summary>
        /// When new partial raw data
        /// </summary>
        public event EventHandler<RecorderEventArgs> RawDataReceived;

        protected void OnStarted()
        {
            Started?.Invoke(this, EventArgs.Empty);
        }


        protected void OnStopped(RecorderEventArgs args)
        {
            Stopped?.Invoke(this, args);
        }

        protected void OnRawDataReceived(IReadOnlyCollection<byte> value)
        {
            RawDataReceived?.Invoke(this, new RecorderEventArgs
            {
                RawData = value,
            });
        }

        #endregion

        #region Public methods

        public virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            IsInitialized = true;

            await Task.Delay(0, cancellationToken);
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (IsStarted)
            {
                throw new InvalidOperationException("Already started");
            }

            IsStarted = true;
            RawData = null;
            WavData = null;

            OnStarted();

            await Task.Delay(0, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken = default)
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

            await Task.Delay(0, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
