using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core.CustomEventArgs;

namespace H.NET.Core
{
    public interface IRecorder : IModule
    {
        #region Properties

        bool IsInitialized { get; }
        bool IsStarted { get; }
        IReadOnlyCollection<byte> RawData { get; }
        IReadOnlyCollection<byte> WavData { get; }
        IReadOnlyCollection<byte> WavHeader { get; }

        #endregion

        #region Events

        event EventHandler Started;
        event EventHandler<RecorderEventArgs> Stopped;
        event EventHandler<RecorderEventArgs> RawDataReceived;

        #endregion

        #region Methods

        Task InitializeAsync(CancellationToken cancellationToken = default);
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
