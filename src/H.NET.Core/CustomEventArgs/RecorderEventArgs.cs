using System;
using System.Collections.Generic;

namespace H.NET.Core.CustomEventArgs
{
    public class RecorderEventArgs : EventArgs
    {
        public IReadOnlyCollection<byte> RawData { get; set; }
        public IReadOnlyCollection<byte> WavData { get; set; }
    }
}
