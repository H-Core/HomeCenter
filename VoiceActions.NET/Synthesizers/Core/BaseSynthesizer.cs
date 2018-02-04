using System;
using System.Threading.Tasks;
using VoiceActions.NET.Utilities;

namespace VoiceActions.NET.Synthesizers.Core
{
    public class BaseSynthesizer : IDisposable
    {
        #region Properties

        public bool UseCache { get; set; } = true;
        public Exception Exception { get; protected set; }

        private ByteArrayCache Cache { get; }

        #endregion

        #region Constructors

        protected BaseSynthesizer()
        {
            Cache = new ByteArrayCache(GetType());
        }

        #endregion

        #region ISynthesizer

        public async Task<byte[]> Convert(string text)
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

        protected virtual async Task<byte[]> InternalConvert(string text) => await Task.Run(() => new byte[0]);

        protected virtual string TextToKey(string text) => text;

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
