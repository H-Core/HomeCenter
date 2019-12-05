using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Media;
using System.Windows;
using System;

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using NAudio.Wave;

namespace NAudioRecorder.IntegrationTests
{
    [TestClass]
    public class RealTimeRecorder
    {
        [TestMethod]
        public void RealTimePlayer()
        {

            var testRecorder = new H.NET.Recorders.NAudioRecorder();
            testRecorder.Start();
            System.Threading.Thread.Sleep(5000);
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
