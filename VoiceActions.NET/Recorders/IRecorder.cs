using System;
using VoiceActions.NET.Recorders.Core;

namespace VoiceActions.NET.Recorders
{
    public interface IRecorder
    {
        bool IsStarted { get; }
        byte[] Data { get; }
        string Text { get; }

        void Start();
        void Stop();

        event EventHandler<RecorderEventArgs> Started;
        event EventHandler<RecorderEventArgs> Stopped;
    }
}
