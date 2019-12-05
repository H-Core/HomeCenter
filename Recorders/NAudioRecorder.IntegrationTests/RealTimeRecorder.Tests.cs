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
        public async Task RealTimePlayer()
        {
            using var testRecorder = new H.NET.Recorders.NAudioRecorder();
            testRecorder.Start();

            await Task.Delay(TimeSpan.FromMilliseconds(5000));

            testRecorder.Stop();

            /*
            {
                await using var output = new FileStream(@"D:\test.wav", FileMode.Create);
                testRecorder.Stream.CopyTo(output);
            }
            //*/

            using (var provider = new RawSourceWaveStream(testRecorder.Stream, testRecorder.WaveFile.WaveFormat))
            //using (var provider = new WaveFileReader(@"D:\test.wav"))
            //using (var provider = new Mp3FileReader(@"c:\benny.mp3"))
            //using (var audioFile = new AudioFileReader(@"c:\benny.mp3"))
            using (var output = new WaveOutEvent())
            {
                output.Init(provider);
                output.Play();
                while (output.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1));
                }
            }
        }
    }
}
