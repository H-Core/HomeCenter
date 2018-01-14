using System.Threading.Tasks;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Tests.Utilities;
using Xunit;

namespace VoiceActions.NET.Tests
{
    public class ConvertersTests : BaseTests
    {
        [Fact]
        public async Task ConvertTest() => await BaseConverterTest(
            "проверка", TestUtilities.GetRawSpeech("speech1.wav"), 
            new WitAiConverter("OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS"));
    }
}
