using System;
using System.Linq;
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
            await recorder.InitializeAsync();

            var provider = new BufferedWaveProvider(new WaveFormat(recorder.Rate, recorder.Bits, recorder.Channels));
            using var output = new WaveOutEvent();
            output.Init(provider);
            output.Play();

            recorder.RawDataReceived += (sender, args) => provider.AddSamples(args.RawData.ToArray(), 0, args.RawData.Count);
            await recorder.StartAsync();
            
            await Task.Delay(TimeSpan.FromMilliseconds(5000));

            await recorder.StopAsync();
        }
    }
}
