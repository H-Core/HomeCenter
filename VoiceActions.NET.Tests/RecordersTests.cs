using System;
using Xunit;
using VoiceActions.NET.Recorders;
using Xunit.Abstractions;

namespace VoiceActions.NET.Tests.Recorders
{
    public class RecordersTests : BaseTests
    {
        public RecordersTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void WinmmRecorderTest() => BaseRecorderTest(
            new WinmmRecorder(), PlatformID.Win32NT);

        [Fact]
        public void AutoStopRecorderTest1() => BaseRecorderTest(
            new AutoStopRecorder(new WinmmRecorder(), 1000), PlatformID.Win32NT);

        [Fact]
        public void AutoStopRecorderTest2() => BaseRecorderTest(
            new AutoStopRecorder(new WinmmRecorder(), 4000), PlatformID.Win32NT);
    }
}
