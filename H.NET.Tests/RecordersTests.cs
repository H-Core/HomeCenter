using System;
using H.NET.Recorders;
using Nito.AsyncEx;
using Xunit;
using Xunit.Abstractions;

namespace H.NET.Tests
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
