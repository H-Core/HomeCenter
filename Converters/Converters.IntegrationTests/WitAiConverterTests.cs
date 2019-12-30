using System.Threading.Tasks;
using H.NET.Core;
using H.NET.Recorders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.NET.Converters.IntegrationTests
{
    [TestClass]
    public class WitAiConverterTests
    {
        public static IRecorder CreateRecorder() => new NAudioRecorder
        {
            Rate = 16000,
        };
        public static IConverter CreateConverter() => new WitAiConverter
        {
            Token = "OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS"
        };

        [TestMethod]
        public async Task StartStreamingRecognitionTest()
        {
            using var converter = CreateConverter();

            await BaseConvertersTests.StartStreamingRecognitionTest(converter, "speech1.wav");
        }

        [TestMethod]
        public async Task StartStreamingRecognitionTest_RealTime()
        {
            using var recorder = CreateRecorder();
            using var converter = CreateConverter();

            await BaseConvertersTests.StartStreamingRecognitionTest_RealTime(recorder, converter);
        }

        [TestMethod]
        public async Task ConvertTest()
        {
            using var converter = CreateConverter();

            await BaseConvertersTests.ConvertTest(converter, "speech1.wav", "проверка");
        }

        [TestMethod]
        public async Task ConvertTest_RealTime()
        {
            using var recorder = CreateRecorder();
            using var converter = CreateConverter();

            await BaseConvertersTests.ConvertTest_RealTime(recorder, converter);
        }
    }
}
