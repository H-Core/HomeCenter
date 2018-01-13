using System;

namespace VoiceActions.NET.Recorders.Core
{
    public class RecorderEventArgs : EventArgs
    {
        public IRecorder SpeechRecorder { get; set; }
        public byte[] Data { get; set; }
        public string Text { get; set; }
    }
}
