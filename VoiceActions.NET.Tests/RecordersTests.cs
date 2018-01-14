using System;
using VoiceActions.NET.Recorders;
using Xunit;
using Xunit.Abstractions;

namespace VoiceActions.NET.Tests
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
            new AutoStopRecorder(new WinmmRecorder(), 500), PlatformID.Win32NT);

        [Fact]
        public void AutoStopRecorderTest2() => BaseRecorderTest(
            new AutoStopRecorder(new WinmmRecorder(), 2000), PlatformID.Win32NT);
    }
}
