using System;

namespace VoiceActions.NET.Recorders
{
    public interface IRecorder
    {
        bool IsStarted { get; }
        byte[] Data { get; }

        void Start();
        void Stop();

        event EventHandler<VoiceActionsEventArgs> Started;
        event EventHandler<VoiceActionsEventArgs> Stopped;
    }
}
