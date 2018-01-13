using System.Threading.Tasks;
using NUnit.Framework;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Tests.Utilities;

namespace VoiceActions.NET.Tests.Converters
{
    [TestFixture]
    public class WitAiConverterTests
    {
        private static async Task BaseTest(string expected, byte[] data, IConverter converter)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(data);
            Assert.IsNotNull(converter);

            Assert.AreEqual(expected, await converter.Convert(data));
        }

        [Test]
        public async Task ConvertTest()
        {
            await BaseTest("проверка", TestUtilities.GetRawSpeech("speech1.wav"), new WitAiConverter("OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS"));
        }
    }
}
