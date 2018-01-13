using Xunit.Abstractions;

namespace VoiceActions.NET.Tests.Utilities
{
    public class OutputTests
    {
        public ITestOutputHelper Output { get; }

        public OutputTests(ITestOutputHelper output)
        {
            Output = output;
        }
    }
}
