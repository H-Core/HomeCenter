using System;
using System.Threading;
using Xunit;
using VoiceActions.NET.Recorders;
using VoiceActions.NET.Tests.Utilities;
using Xunit.Abstractions;

namespace VoiceActions.NET.Tests.Recorders
{
    public class WitmmRecorderTests : OutputTests
    {
        public WitmmRecorderTests(ITestOutputHelper output) : base(output)
        {
        }

        private void BaseTest(IRecorder recorder, PlatformID? platformId = null)
        {
            Assert.NotNull(recorder);
            if (platformId != null && platformId != Environment.OSVersion.Platform)
            {
                Output.WriteLine($"Recorder: {recorder} not support current system: {Environment.OSVersion}");
                return;
            }

            recorder.Start();
            Thread.Sleep(2000);
            recorder.Stop();

            Assert.NotNull(recorder.Data);
            Assert.InRange(recorder.Data.Length, 1, int.MaxValue);

            // Check double disposing
            recorder.Dispose();
            recorder.Dispose();

            Output.WriteLine($"Recorder: {recorder} is good!");
        }

        [Fact]
        public void WinmmRecorderTest() => BaseTest(new WinmmRecorder(), PlatformID.Win32NT); // need to test in the main thread

        [Fact]
        public void AutoStopRecorderTest1() => BaseTest(new AutoStopRecorder<WinmmRecorder>(1000), PlatformID.Win32NT); // need to test in the main thread

        [Fact]
        public void AutoStopRecorderTest2() => BaseTest(new AutoStopRecorder<WinmmRecorder>(4000), PlatformID.Win32NT); // need to test in the main thread
    }
}
