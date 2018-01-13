using System;
using Xunit;
using VoiceActions.NET.Recorders;
using Xunit.Abstractions;

namespace VoiceActions.NET.Tests.Recorders
{
    public class WitmmRecorderTests : BaseTests
    {
        public WitmmRecorderTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void WinmmRecorderTest() => BaseRecorderTest(
            new WinmmRecorder(), PlatformID.Win32NT); // need to test in the main thread

        [Fact]
        public void AutoStopRecorderTest1() => BaseRecorderTest(
            new AutoStopRecorder<WinmmRecorder>(1000), PlatformID.Win32NT); // need to test in the main thread

        [Fact]
        public void AutoStopRecorderTest2() => BaseRecorderTest(
            new AutoStopRecorder<WinmmRecorder>(4000), PlatformID.Win32NT); // need to test in the main thread
    }
}
