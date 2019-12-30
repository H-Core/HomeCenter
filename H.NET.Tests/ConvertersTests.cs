using System;
using System.Threading.Tasks;
using H.NET.Converters;
using H.NET.Tests.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace H.NET.Tests
{
    public class ConvertersTests : BaseTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConvertersTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task WitAiConverterTest() => await BaseConverterTest(
            "проверка", ResourcesUtilities.ReadFileAsBytes("speech1.wav"), 
            new WitAiConverter
            {
                Token = "OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS"
            });

        [Fact]
        public async Task StartStreamingRecognitionTest()
        {
            using var converter = new WitAiConverter
            {
                Token = "OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS"
            };

            using var recognition = await converter.StartStreamingRecognitionAsync();
            recognition.AfterPartialResults += (sender, args) => _testOutputHelper.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterPartialResults: {args.Text}");
            recognition.AfterFinalResults += (sender, args) => _testOutputHelper.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterFinalResults: {args.Text}");

            var bytes = ResourcesUtilities.ReadFileAsBytes("speech1.wav");
            const int bytesPerWrite = 8000;
            for (var i = 0; i < bytes.Length; i += bytesPerWrite)
            {
                var chunk = new ArraySegment<byte>(bytes, i, i < bytes.Length - bytesPerWrite ? bytesPerWrite : bytes.Length - i).ToArray();
                await recognition.WriteAsync(chunk);

                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }

            await recognition.StopAsync();
        }
    }
}
