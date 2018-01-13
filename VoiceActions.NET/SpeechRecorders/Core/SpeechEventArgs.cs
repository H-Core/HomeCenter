using System;

namespace VoiceActions.NET.SpeechRecorders.Core
{
    public class SpeechEventArgs : EventArgs
    {
        public ISpeechRecorder SpeechRecorder { get; set; }
        public byte[] Data { get; set; }
        public string Text { get; set; }
    }
}
