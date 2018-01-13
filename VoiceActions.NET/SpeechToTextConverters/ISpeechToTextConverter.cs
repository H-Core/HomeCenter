using System.Threading.Tasks;

namespace VoiceActions.NET.SpeechToTextConverters
{
    public interface ISpeechToTextConverter
    {
        Task<string> Convert(byte[] bytes);
    }
}
