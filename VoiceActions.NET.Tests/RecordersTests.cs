using System;
using Nito.AsyncEx;
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
        public void WinmmRecorderTest() => 
            AsyncContext.Run(async () => await BaseRecorderTest(new WinmmRecorder(), PlatformID.Win32NT));
    }
}
