using System;
using VoiceActions.NET.SpeechRecorders.Core;

namespace VoiceActions.NET.SpeechRecorders
{
    public interface ISpeechRecorder
    {
        bool IsStarted { get; }
        byte[] Data { get; }
        string Text { get; }

        void Start();
        void Stop();

        event EventHandler<SpeechEventArgs> Started;
        event EventHandler<SpeechEventArgs> Stopped;
    }
}
