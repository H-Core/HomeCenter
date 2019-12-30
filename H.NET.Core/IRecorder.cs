using System;
using System.Collections.Generic;

namespace H.NET.Core
{
    public interface IRecorder : IModule
    {
        bool IsStarted { get; }
        IReadOnlyCollection<byte> Data { get; }

        void Start();
        void Stop();

        event EventHandler<VoiceActionsEventArgs> Started;
        event EventHandler<VoiceActionsEventArgs> Stopped; 
        event EventHandler<VoiceActionsEventArgs> NewData;
    }
}
