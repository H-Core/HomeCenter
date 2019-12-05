using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAudio.Wave;

namespace NAudioRecorder.IntegrationTests
{
    [TestClass]
    public class RealTimeRecorder
    {
        [TestMethod]
        public async Task RealTimePlayRecordTest()
        {
            using var recorder = new H.NET.Recorders.NAudioRecorder();
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
