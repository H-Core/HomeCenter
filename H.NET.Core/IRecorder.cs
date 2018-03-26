using System;

namespace H.NET.Core
{
    public interface IRecorder : IDisposable
    {
        bool IsStarted { get; }
        byte[] Data { get; }

        void Start();
        void Stop();

        event EventHandler<VoiceActionsEventArgs> Started;
        event EventHandler<VoiceActionsEventArgs> Stopped;
    }
}
