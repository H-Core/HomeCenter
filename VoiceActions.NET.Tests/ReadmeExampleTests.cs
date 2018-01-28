using System;
using System.Threading;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Recorders;
using Xunit;
using Xunit.Abstractions;

namespace VoiceActions.NET.Tests
{
    public class ReadmeExampleTests
    {
        private ITestOutputHelper Output { get; }

        public ReadmeExampleTests(ITestOutputHelper output)
        {
            Output = output;
        }

        private static ActionsManager CreateExampleManager()
        {
            var manager = new ActionsManager
            {
                // Select recorder which stops after 1000 milliseconds with Windows Multimedia API base recorder
                Recorder = new AutoStopRecorder(new WinmmRecorder(), 1000),
                // Select Wit.ai voice-to-text converter
                Converter = new WitAiConverter("OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS")
            };

            // when you say "test" the manager runs the explorer.exe with the "C:/" base folder
            manager.SetCommand("test", "run explorer.exe C:/");
            // when you say "test" the manager runs your custom action
            manager.SetAction("test", () => Console.WriteLine("test"));

            Assert.Equal("run explorer.exe C:/", manager.GetCommand("test"));
            Assert.Single(manager.GetCommands());
            Assert.NotNull(manager.GetAction("test"));
            Assert.Single(manager.GetActions());

            return manager;
        }

        [Fact]
        public void ReadmeExampleAutoStopTest()
        {
            var manager = CreateExampleManager();

            if (!BaseTests.CheckPlatform(PlatformID.Win32NT))
            {
                Output?.WriteLine($"Current system is not supported: {Environment.OSVersion}");
                return;
            }

            // Start the recording process. It stops after 1 second (if AutoStopRecorder is selected from the example)
            Assert.False(manager.IsStarted);
            manager.Start();
            Assert.True(manager.IsStarted);

            Thread.Sleep(2000);

            Assert.False(manager.IsStarted);
        }

        [Fact]
        public void ReadmeExampleWithoutAutoStopTest()
        {
            var manager = CreateExampleManager();

            if (!BaseTests.CheckPlatform(PlatformID.Win32NT))
            {
                Output?.WriteLine($"Current system is not supported: {Environment.OSVersion}");
                return;
            }

            // Start the recording process without autostop
            Assert.False(manager.IsStarted);
            manager.StartWithoutAutostop();
            Assert.True(manager.IsStarted);

            Thread.Sleep(2000);

            Assert.True(manager.IsStarted);
            manager.Stop();
            Assert.False(manager.IsStarted);
        }

        [Fact]
        public void ReadmeExampleChangeTest()
        {
            var manager = CreateExampleManager();

            if (!BaseTests.CheckPlatform(PlatformID.Win32NT))
            {
                Output?.WriteLine($"Current system is not supported: {Environment.OSVersion}");
                return;
            }

            // The first run will start the recording process, the second will leave the recording process and start the action
            Assert.False(manager.IsStarted);
            manager.Change();
            Assert.True(manager.IsStarted);

            Thread.Sleep(2000);

            Assert.False(manager.IsStarted);
        }

        [Fact]
        public void ReadmeExampleChangeWithoutAutoStopTest()
        {
            var manager = CreateExampleManager();

            if (!BaseTests.CheckPlatform(PlatformID.Win32NT))
            {
                Output?.WriteLine($"Current system is not supported: {Environment.OSVersion}");
                return;
            }

            // The first run will start the recording process, the second will leave the recording process and start the action. Auto stop is disabled
            Assert.False(manager.IsStarted);
            manager.ChangeWithoutAutostop();
            Assert.True(manager.IsStarted);

            Thread.Sleep(2000);

            Assert.True(manager.IsStarted);
            manager.Change();
            Assert.False(manager.IsStarted);
        }
    }
}
