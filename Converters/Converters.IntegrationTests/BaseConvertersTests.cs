using System;
using System.Linq;
using System.Threading.Tasks;
using H.NET.Converters.IntegrationTests.Utilities;
using H.NET.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.NET.Converters.IntegrationTests
{
    public static class BaseConvertersTests
    {
        public static async Task StartStreamingRecognitionTest(IConverter converter, string name, int bytesPerWrite = 8000)
        {
            using var recognition = await converter.StartStreamingRecognitionAsync();
            recognition.AfterPartialResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterPartialResults: {args.Text}");
            recognition.AfterFinalResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterFinalResults: {args.Text}");

            var bytes = ResourcesUtilities.ReadFileAsBytes(name);
            for (var i = 0; i < bytes.Length; i += bytesPerWrite)
            {
                var chunk = new ArraySegment<byte>(bytes, i, i < bytes.Length - bytesPerWrite ? bytesPerWrite : bytes.Length - i).ToArray();
                await recognition.WriteAsync(chunk);

                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }

            await recognition.StopAsync();
        }

        public static async Task StartStreamingRecognitionTest_RealTime(IRecorder recorder, IConverter converter)
        {
            recorder.Start();

            using var recognition = await converter.StartStreamingRecognitionAsync();
            recognition.AfterPartialResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterPartialResults: {args.Text}");
            recognition.AfterFinalResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterFinalResults: {args.Text}");

            if (recorder.Data != null)
            {
                await recognition.WriteAsync(recorder.Data.ToArray());
            }

            // ReSharper disable once AccessToDisposedClosure
            recorder.NewData += async (sender, args) => await recognition.WriteAsync(args.Data.ToArray());

            await Task.Delay(TimeSpan.FromMilliseconds(5000));

            recorder.Stop();
            await recognition.StopAsync();
        }

        public static async Task ConvertTest(IConverter converter, string name, string expected)
        {
            var bytes = ResourcesUtilities.ReadFileAsBytes(name);
            var actual = await converter.ConvertAsync(bytes);

            Assert.AreEqual(expected, actual);
        }

        public static async Task ConvertTest_RealTime(IRecorder recorder, IConverter converter)
        {
            recorder.Start();

            await Task.Delay(TimeSpan.FromMilliseconds(5000));

            recorder.Stop();

            var bytes = recorder.Data;

            var result = await converter.ConvertAsync(bytes.ToArray());

            Console.WriteLine(result);
        }
    }
}
