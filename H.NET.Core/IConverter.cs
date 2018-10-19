using System.Threading.Tasks;

namespace H.NET.Core
{
    public interface IConverter : IModule
    {
        Task<string> Convert(byte[] bytes);
    }
}
