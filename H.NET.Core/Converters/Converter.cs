using System;
using System.Threading.Tasks;

namespace H.NET.Core.Converters
{
    public class Converter : Module, IConverter
    {
        #region Properties

        public Exception Exception { get; protected set; }

        public virtual Task<string> Convert(byte[] bytes) => null;

        #endregion
    }
}
