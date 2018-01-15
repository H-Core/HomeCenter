using System;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Recorders;

namespace VoiceActions.NET
{
    public class VoiceActionsEventArgs : EventArgs
    {
        public IRecorder Recorder { get; set; }
        public IConverter Converter { get; set; }
        public byte[] Data { get; set; }
        public string Text { get; set; }
    }
}
