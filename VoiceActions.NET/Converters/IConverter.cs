using System.Threading.Tasks;

namespace VoiceActions.NET.Converters
{
    public interface IConverter
    {
        Task<string> Convert(byte[] bytes);
    }
}
