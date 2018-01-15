using System;
using System.Threading.Tasks;

namespace VoiceActions.NET.Converters
{
    public interface IConverter : IDisposable
    {
        Exception Exception { get; }

        Task<string> Convert(byte[] bytes);
    }
}
