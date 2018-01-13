using System.Threading;
using NUnit.Framework;
using VoiceActions.NET.Recorders;

namespace VoiceActions.NET.Tests.Recorders
{
    [TestFixture]
    public class WitmmSpeechRecorderTests
    {
        private static void BaseTest(IRecorder recorder)
        {
            Assert.IsNotNull(recorder, "Recorder is null");

            recorder.Start();
            Thread.Sleep(2000);
            recorder.Stop();

            Assert.IsNotNull(recorder.Data, "Recorder Data is null");
            Assert.Greater(recorder.Data.Length, 0, "Recorder Data is empty");

            Assert.Pass($"Recorder: {recorder} is good!");
        }

        [Test]
        public void WinmmSpeechRecorderTest() => BaseTest(new WinmmRecorder()); // need to test in the main thread

        [Test]
        public void AutoStopSpeechRecorderTest1() => BaseTest(new AutoStopRecorder<WinmmRecorder>(1000)); // need to test in the main thread

        [Test]
        public void AutoStopSpeechRecorderTest2() => BaseTest(new AutoStopRecorder<WinmmRecorder>(4000)); // need to test in the main thread
    }
}
