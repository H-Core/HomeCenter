using System;
using System.Threading.Tasks;

namespace H.NET.Core
{
    public interface ISynthesizer : IDisposable
    {
        Exception Exception { get; }
        bool UseCache { get; set; }

        Task<byte[]> Convert(string text);
    }
}
