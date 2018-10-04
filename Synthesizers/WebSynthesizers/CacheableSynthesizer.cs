using System.Threading.Tasks;
using H.NET.Core.Synthesizers;

namespace H.NET.Synthesizers
{
    public class CacheableSynthesizer : Synthesizer
    {
        #region Properties

        private ByteArrayCache Cache { get; }

        #endregion

        #region Constructors

        protected CacheableSynthesizer()
        {
            UseCache = true;
            Cache = new ByteArrayCache(GetType());
        }

        #endregion

        #region ISynthesizer

        public new async Task<byte[]> Convert(string text)
        {
            var key = TextToKey(text);
            if (UseCache && Cache.Contains(key))
            {
                return Cache[key];
            }

            var bytes = await InternalConvert(text);
            Cache[key] = bytes;
            return bytes;
        }

        protected virtual string TextToKey(string text) => text;

        #endregion
    }
}
