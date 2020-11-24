using System;
using H.NET.Core.Managers;
using H.NET.Recorders;
using Nito.AsyncEx;
using Xunit;
using Xunit.Abstractions;

namespace H.NET.Tests
{
    public class ManagersTests : BaseTests
    {
        public ManagersTests(ITestOutputHelper output) : base(output)
        {
        }
        /*
        [Fact]
        public void WinmmWitAiManagerTest() =>
            AsyncContext.Run(async () => await BaseManagerTest(new BaseManager
            {
                Recorder = new WinmmRecorder(),
                Converter = new WitAiConverter
                {
                    Token = "OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS"
                }
            }, PlatformID.Win32NT));

        [Fact]
        public void WinmmYandexManagerTest() =>
            AsyncContext.Run(async () => await BaseManagerTest(new BaseManager
            {
                Recorder = new WinmmRecorder(),
                Converter = new YandexConverter
                {
                    Key = "1ce29818-0d15-4080-b6a1-ea5267c9fefd",
                    Lang = "ru-RU",
                    Topic = "queries"
                }
            }, PlatformID.Win32NT));*/
    }
}
