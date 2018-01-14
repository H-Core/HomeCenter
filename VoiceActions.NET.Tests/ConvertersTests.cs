using System.Threading.Tasks;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Tests.Utilities;
using Xunit;

namespace VoiceActions.NET.Tests
{
    public class ConvertersTests : BaseTests
    {
        [Fact]
        public async Task WitAiConverterTest() => await BaseConverterTest(
            "проверка", TestUtilities.GetRawSpeech("speech1.wav"), 
            new WitAiConverter("OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS"));

        [Fact]
        public async Task YandexConverterTest() => await BaseConverterTest(
            "проверка", TestUtilities.GetRawSpeech("speech1.wav"),
            new YandexConverter("1ce29818-0d15-4080-b6a1-ea5267c9fefd")
            {
                Lang = "ru-RU",
                Topic = "queries"
            });
    }
}
