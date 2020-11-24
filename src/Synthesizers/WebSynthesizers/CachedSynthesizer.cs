using System.Threading;
using System.Threading.Tasks;
using H.NET.Core;
using H.NET.Core.Synthesizers;

namespace H.NET.Synthesizers
{
    public abstract class CachedSynthesizer : Synthesizer, ISynthesizer
    {
        #region Properties

        private ByteArrayCache Cache { get; }

        #endregion

        #region Constructors

        protected CachedSynthesizer()
        {
            UseCache = true;
            Cache = new ByteArrayCache(GetType());
        }

        #endregion

        #region ISynthesizer

        public async Task<byte[]> ConvertAsync(string text, CancellationToken cancellationToken = default)
        {
            var key = TextToKey(text);
            if (UseCache && Cache.Contains(key))
            {
                return Cache[key];
            }

            var bytes = await InternalConvertAsync(text, cancellationToken);
            Cache[key] = bytes;
            return bytes;
        }

        protected abstract Task<byte[]> InternalConvertAsync(string text, CancellationToken cancellationToken = default);

        protected abstract string TextToKey(string text);

        #endregion
    }
}
