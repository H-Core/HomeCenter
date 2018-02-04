using System;
using System.Threading.Tasks;

namespace VoiceActions.NET.Synthesizers
{
    public interface ISynthesizer : IDisposable
    {
        Exception Exception { get; }

        Task<byte[]> Convert(string text);
    }
}
