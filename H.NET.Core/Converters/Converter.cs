using System.Threading.Tasks;

namespace H.NET.Core.Converters
{
    public class Converter : Module, IConverter
    {
        #region Properties

        public virtual Task<string> Convert(byte[] bytes) => null;

        #endregion
    }
}
