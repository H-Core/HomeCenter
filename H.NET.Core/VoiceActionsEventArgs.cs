using System;

namespace H.NET.Core
{
    public class VoiceActionsEventArgs : EventArgs
    {
        public IRecorder Recorder { get; set; }
        public IConverter Converter { get; set; }
        public byte[] Data { get; set; }
        public string Text { get; set; }
    }
}
