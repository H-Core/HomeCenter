using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAudio.Wave;

namespace NAudioRecorder.IntegrationTests
{
    [TestClass]
    public class RealTimeRecorder
    {
        [TestMethod]
        public async Task RealTimePlayer()
        {
            using (var audioFile = new AudioFileReader(@"c:\benny.mp3"))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1000));
                }
            }

            var testRecorder = new H.NET.Recorders.NAudioRecorder();
            testRecorder.Start();

            await Task.Delay(TimeSpan.FromMilliseconds(5000));

            testRecorder.Stop();
            IWaveProvider provider = new RawSourceWaveStream(
                testRecorder.Stream, new WaveFormat());
            using (var waveOut = new WaveOutEvent())
            //using (var wavReader = new WaveFileReader(@"c:\benny.wav"))
            using (var wavReader = new Mp3FileReader(@"c:\benny.mp3"))
            {
                waveOut.Init(wavReader);
                waveOut.Play();
            }
            Assert.AreEqual(true, true);
        }
    }
}
