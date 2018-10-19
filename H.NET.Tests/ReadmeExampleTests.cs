using System;
using System.Diagnostics;
using System.Threading.Tasks;
using H.NET.Converters;
using H.NET.Core.Managers;
using H.NET.Core.Recorders;
using H.NET.Recorders;
using Nito.AsyncEx;
using Xunit;
using Xunit.Abstractions;

namespace H.NET.Tests
{
    public class ReadmeExampleTests
    {
        private ITestOutputHelper Output { get; }

        public ReadmeExampleTests(ITestOutputHelper output)
        {
            Output = output;
        }

        private static Manager<Action> CreateExampleManager()
        {
            var manager = new Manager<Action>
            {
                // Select recorder which stops after 1000 milliseconds with Windows Multimedia API base recorder
                Recorder = new WinmmRecorder(),
                // Select Wit.ai voice-to-text converter
                Converter = new WitAiConverter
                {
                    Token = "OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS"
                }
            };
            manager.NewValue += (key, value) => value?.Invoke();

            // when you say "open file explorer" the manager runs the explorer.exe with the "C:/" base folder
            manager.Storage["open file explorer"] = () => Process.Start("explorer.exe", "C:/");
            // when you say any text(include empty text) the manager runs your custom action
            manager.NewText += text => Console.WriteLine($"You say: {text}");

            Assert.Single(manager.Storage);
            Assert.True(manager.Storage.ContainsKey("open file explorer"));
            
            Assert.True(manager.Storage.ContainsKey("open file explorer"));
            Assert.False(manager.Storage.ContainsKey("any text"));
            Assert.False(manager.Storage.ContainsKey(""));
            Assert.Throws<ArgumentNullException>(() => manager.Storage.ContainsKey(null));

            return manager;
        }

        private bool CheckPlatform()
        {
            if (!BaseTests.CheckPlatform(PlatformID.Win32NT))
            {
                Output?.WriteLine($"Current system is not supported: {Environment.OSVersion}");
                return false;
            }

            return true;
        }
        /*
        [Fact]
        public void ReadmeExampleTest() => AsyncContext.Run(async () =>
        {
            var manager = CreateExampleManager();
            if (!CheckPlatform())
            {
                return;
            }

            // Start the recording process. It stops after 1 second (if AutoStopRecorder is selected from the example)
            Assert.False(manager.IsStarted);
            manager.Start();
            Assert.True(manager.IsStarted);

            await Task.Delay(2000);

            Assert.True(manager.IsStarted);
            manager.Stop();
            Assert.False(manager.IsStarted);
        });

        [Fact]
        public void ReadmeExampleWithTimeoutTest() => AsyncContext.Run(async () =>
        {
            var manager = CreateExampleManager();
            if (!CheckPlatform())
            {
                return;
            }

            // Start the recording process without autostop
            Assert.False(manager.IsStarted);
            manager.StartWithTimeout(1000);
            Assert.True(manager.IsStarted);

            await Task.Delay(2000);

            Assert.False(manager.IsStarted);
        });

        [Fact]
        public void ReadmeExampleChangeTest() => AsyncContext.Run(async () =>
        {
            var manager = CreateExampleManager();
            if (!CheckPlatform())
            {
                return;
            }

            // The first run will start the recording process, the second will leave the recording process and start the action
            Assert.False(manager.IsStarted);
            manager.Change();
            Assert.True(manager.IsStarted);

            await Task.Delay(2000);

            Assert.True(manager.IsStarted);
            manager.Change();
            Assert.False(manager.IsStarted);
        });

        [Fact]
        public void ReadmeExampleChangeWithTimeoutTest() => AsyncContext.Run(async () =>
        {
            var manager = CreateExampleManager();
            if (!CheckPlatform())
            {
                return;
            }

            // The first run will start the recording process, the second will leave the recording process and start the action. Auto stop is disabled
            Assert.False(manager.IsStarted);
            manager.ChangeWithTimeout(1000);
            Assert.True(manager.IsStarted);

            await Task.Delay(2000);

            Assert.False(manager.IsStarted);
        });*/
    }
}
