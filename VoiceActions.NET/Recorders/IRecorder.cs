using System;

namespace VoiceActions.NET.Recorders
{
    public interface IRecorder : IDisposable
    {
        bool IsStarted { get; }
        byte[] Data { get; }

        void Start();
        void Stop();

        event EventHandler<VoiceActionsEventArgs> Started;
        event EventHandler<VoiceActionsEventArgs> Stopped;

        bool AutoStopEnabled { get; set; }
        int Interval { get; }
    }
}
