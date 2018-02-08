using System;
using Nito.AsyncEx;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Managers;
using VoiceActions.NET.Recorders;
using Xunit;
using Xunit.Abstractions;

namespace VoiceActions.NET.Tests
{
    public class ManagersTests : BaseTests
    {
        public ManagersTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void WinmmWitAiManagerTest() =>
            AsyncContext.Run(async () => await BaseManagerTest(new BaseManager
            {
                Recorder = new WinmmRecorder(),
                Converter = new WitAiConverter("OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS")
            }, PlatformID.Win32NT));

        [Fact]
        public void WinmmYandexManagerTest() =>
            AsyncContext.Run(async () => await BaseManagerTest(new BaseManager
            {
                Recorder = new WinmmRecorder(),
                Converter = new YandexConverter("1ce29818-0d15-4080-b6a1-ea5267c9fefd")
                {
                    Lang = "ru-RU",
                    Topic = "queries"
                }
            }, PlatformID.Win32NT));
    }
}
