using System;
using System.Collections.Generic;
using H.NET.Core.CustomEventArgs;

namespace H.NET.Core
{
    public interface IRecorder : IModule
    {
        bool IsStarted { get; }
        IReadOnlyCollection<byte> RawData { get; }
        IReadOnlyCollection<byte> WavData { get; }

        void Start();
        void Stop();

        event EventHandler Started;
        event EventHandler<RecorderEventArgs> Stopped; 
        event EventHandler<RecorderEventArgs> NewRawData;
    }
}
