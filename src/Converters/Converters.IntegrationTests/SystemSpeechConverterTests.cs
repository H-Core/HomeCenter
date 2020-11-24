#if NETFRAMEWORK
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.NET.Converters.IntegrationTests
{

    [TestClass]
    public class SystemSpeechConverterTests
    {
        [TestMethod]
        public async Task StartStreamingRecognitionTest_RealTime()
        {
            using var converter = new SystemSpeechConverter();

            using var recognition = await converter.StartStreamingRecognitionAsync();
            recognition.AfterPartialResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterPartialResults: {args.Text}");
            recognition.AfterFinalResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterFinalResults: {args.Text}");

            await Task.Delay(TimeSpan.FromSeconds(5));

            await recognition.StopAsync();
        }
    }
}
#endif
