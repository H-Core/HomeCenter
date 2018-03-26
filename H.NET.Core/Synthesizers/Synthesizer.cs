using System;
using System.Threading.Tasks;

namespace H.NET.Core.Synthesizers
{
    public class Synthesizer : Module, ISynthesizer
    {
        #region Properties

        public bool UseCache { get; set; } = false;
        public Exception Exception { get; protected set; }

        #endregion

        #region Constructors

        protected Synthesizer()
        {
        }

        #endregion

        #region ISynthesizer

        public async Task<byte[]> Convert(string text)
        {
            return await InternalConvert(text);
        }

        protected virtual async Task<byte[]> InternalConvert(string text) => await Task.Run(() => new byte[0]);

        #endregion
    }
}
