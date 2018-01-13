using System;
using System.Threading.Tasks;

namespace VoiceActions.NET.Converters
{
    public interface IConverter : IDisposable
    {
        Task<string> Convert(byte[] bytes);
    }
}
