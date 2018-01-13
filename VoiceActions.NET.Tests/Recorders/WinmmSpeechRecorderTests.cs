using System.Threading;
using Xunit;
using VoiceActions.NET.Recorders;
using VoiceActions.NET.Tests.Utilities;
using Xunit.Abstractions;

namespace VoiceActions.NET.Tests.Recorders
{
    public class WitmmSpeechRecorderTests : OutputTests
    {
        public WitmmSpeechRecorderTests(ITestOutputHelper output) : base(output)
        {
        }

        private void BaseTest(IRecorder recorder)
        {
            Assert.NotNull(recorder);

            recorder.Start();
            Thread.Sleep(2000);
            recorder.Stop();

            Assert.NotNull(recorder.Data);
            Assert.InRange(recorder.Data.Length, 1, int.MaxValue);

            Output.WriteLine($"Recorder: {recorder} is good!");
        }

        [Fact]
        public void WinmmSpeechRecorderTest() => BaseTest(new WinmmRecorder()); // need to test in the main thread

        [Fact]
        public void AutoStopSpeechRecorderTest1() => BaseTest(new AutoStopRecorder<WinmmRecorder>(1000)); // need to test in the main thread

        [Fact]
        public void AutoStopSpeechRecorderTest2() => BaseTest(new AutoStopRecorder<WinmmRecorder>(4000)); // need to test in the main thread
    }
}
