using System.IO;

namespace H.NET.Utilities.Plugins.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToArray(this Stream stream)
        {
            using var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }
    }
}
