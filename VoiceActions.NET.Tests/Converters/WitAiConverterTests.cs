using System.Threading.Tasks;
using Xunit;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Tests.Utilities;

namespace VoiceActions.NET.Tests.Converters
{
    public class WitAiConverterTests
    {
        private static async Task BaseTest(string expected, byte[] data, IConverter converter)
        {
            Assert.NotNull(expected);
            Assert.NotNull(data);
            Assert.NotNull(converter);

            Assert.Equal(expected, await converter.Convert(data));

            // Check double disposing
            converter.Dispose();
            converter.Dispose();
        }

        [Fact]
        public async Task ConvertTest()
        {
            await BaseTest("проверка", TestUtilities.GetRawSpeech("speech1.wav"), new WitAiConverter("OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS"));
        }
    }
}
