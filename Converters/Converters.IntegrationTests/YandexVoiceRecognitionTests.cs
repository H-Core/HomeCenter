using System;
using System.Threading.Tasks;
using H.NET.Converters.IntegrationTests.Utilities;
using H.NET.Recorders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.NET.Converters.IntegrationTests
{
    [TestClass]
    public class YandexVoiceRecognitionTests
    {
        public const string FolderId = "$FolderId$";
        public const string OAuthToken = "$OAuthToken$";

        [TestMethod]
        public async Task StartStreamingRecognitionTest()
        {
            using var converter = new YandexConverter
            {
                OAuthToken = OAuthToken,
                FolderId = FolderId,
                Lang = "ru-RU",
                SampleRateHertz = 8000,
            };

            using var recognition = await converter.StartStreamingRecognitionAsync();
            recognition.AfterPartialResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterPartialResults: {args.Text}");
            recognition.AfterFinalResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterFinalResults: {args.Text}");

            var bytes = ResourcesUtilities.ReadFileAsBytes("проверка_проверка_8000.wav");
            const int bytesPerWrite = 8000;
            for (var i = 0; i < bytes.Length; i += bytesPerWrite)
            {
                var chunk = new ArraySegment<byte>(bytes, i, i < bytes.Length - bytesPerWrite ? bytesPerWrite : bytes.Length - i).ToArray();
                await recognition.WriteAsync(chunk);

                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }

            await recognition.StopAsync();
        }

        [TestMethod]
        public async Task StartStreamingRecognitionTest_RealTime()
        {
            using var recorder = new NAudioRecorder();
            recorder.Start();

            using var converter = new YandexConverter
            {
                OAuthToken = OAuthToken,
                FolderId = FolderId,
                Lang = "ru-RU",
                SampleRateHertz = 8000,
            };

            using var recognition = await converter.StartStreamingRecognitionAsync();
            recognition.AfterPartialResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterPartialResults: {args.Text}");
            recognition.AfterFinalResults += (sender, args) => Console.WriteLine($"{DateTime.Now:h:mm:ss.fff} AfterFinalResults: {args.Text}");

            await recognition.WriteAsync(recorder.Data);

            // ReSharper disable once AccessToDisposedClosure
            recorder.NewData += async (sender, args) => await recognition.WriteAsync(args.Buffer);

            await Task.Delay(TimeSpan.FromMilliseconds(5000));

            recorder.Stop();
            await recognition.StopAsync();
        }

        [TestMethod]
        public async Task ConvertTest()
        {
            using var converter = new YandexConverter
            {
                OAuthToken = OAuthToken,
                FolderId = FolderId,
                Lang = "ru-RU",
                SampleRateHertz = 8000,
            };

            var bytes = ResourcesUtilities.ReadFileAsBytes("проверка_проверка_8000.wav");
            var result = await converter.ConvertAsync(bytes);

            Assert.AreEqual("проверка проверка", result);
        }

        [TestMethod]
        public async Task ConvertTest_RealTime()
        {
            using var recorder = new NAudioRecorder();
            recorder.Start();

            await Task.Delay(TimeSpan.FromMilliseconds(5000));

            recorder.Stop();

            var bytes = recorder.Data;

            using var converter = new YandexConverter
            {
                OAuthToken = OAuthToken,
                FolderId = FolderId,
                Lang = "ru-RU",
                SampleRateHertz = 8000,
            };
            var result = await converter.ConvertAsync(bytes);

            Console.WriteLine(result);
        }
    }
}
