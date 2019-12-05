using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAudio.Wave;

namespace H.NET.Recorders.IntegrationTests
{
    [TestClass]
    public class RealTimeRecorder
    {
        [TestMethod]
        public async Task RealTimePlayRecordTest()
        {
            Console.WriteLine("Available devices:");
            foreach (var device in NAudioRecorder.GetAvailableDevices())
            {
                Console.WriteLine($" - Name: {device.Name}, Channels: {device.Channels}");
            }

            using var recorder = new NAudioRecorder();
            var provider = new BufferedWaveProvider(recorder.WaveIn.WaveFormat);
            using var output = new WaveOutEvent();
            output.Init(provider);
            output.Play();

            recorder.NewData += (sender, args) => provider.AddSamples(args.Buffer, 0, args.BytesRecorded);
            recorder.Start();

            await Task.Delay(TimeSpan.FromMilliseconds(5000));

            recorder.Stop();

        }
    }
}
