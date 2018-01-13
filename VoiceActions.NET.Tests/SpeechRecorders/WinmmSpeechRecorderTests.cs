using System.Threading;
using NUnit.Framework;
using VoiceActions.NET.SpeechRecorders;

namespace VoiceActions.NET.Tests.SpeechRecorders
{
    [TestFixture]
    public class WitmmSpeechRecorderTests
    {
        private static void BaseTest(ISpeechRecorder recorder)
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
        public void WinmmSpeechRecorderTest() => BaseTest(new WinmmSpeechRecorder()); // need to test in the main thread

        [Test]
        public void AutoStopSpeechRecorderTest1() => BaseTest(new AutoStopSpeechRecorder<WinmmSpeechRecorder>(1000)); // need to test in the main thread

        [Test]
        public void AutoStopSpeechRecorderTest2() => BaseTest(new AutoStopSpeechRecorder<WinmmSpeechRecorder>(4000)); // need to test in the main thread
    }
}
