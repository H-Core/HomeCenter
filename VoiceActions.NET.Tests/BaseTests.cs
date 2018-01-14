using System;
using System.Threading;
using System.Threading.Tasks;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Recorders;
using Xunit;
using Xunit.Abstractions;

namespace VoiceActions.NET.Tests
{
    public class BaseTests
    {
        protected ITestOutputHelper Output { get; }

        protected BaseTests()
        {
        }

        protected BaseTests(ITestOutputHelper output)
        {
            Output = output;
        }

        protected static void BaseDisposeTest(IDisposable obj)
        {
            // Check double disposing
            obj.Dispose();
            obj.Dispose();
        }

        protected bool CheckPlatform(PlatformID? platformId) => 
            platformId == null || platformId == Environment.OSVersion.Platform;

        protected void BaseRecorderTest(IRecorder recorder, PlatformID? platformId = null)
        {
            Assert.NotNull(recorder);
            if (!CheckPlatform(platformId))
            {
                Output?.WriteLine($"Recorder: {recorder} not support current system: {Environment.OSVersion}");
                return;
            }

            recorder.Start();
            Thread.Sleep(2000);
            recorder.Stop();

            Assert.NotNull(recorder.Data);
            Assert.InRange(recorder.Data.Length, 1, int.MaxValue);

            BaseDisposeTest(recorder);

            Output?.WriteLine($"Recorder: {recorder} is good!");
        }

        protected static async Task BaseConverterTest(string expected, byte[] data, IConverter converter)
        {
            Assert.NotNull(expected);
            Assert.NotNull(data);
            Assert.NotNull(converter);

            Assert.Equal(expected, await converter.Convert(data));

            BaseDisposeTest(converter);
        }

        protected void BaseVoiceManagerTest(VoiceManager manager, PlatformID? platformId = null)
        {
            Assert.NotNull(manager);
            if (!CheckPlatform(platformId))
            {
                Output?.WriteLine($"Manager: {manager} not support current system: {Environment.OSVersion}");
                return;
            }

            var startedEvent = new AutoResetEvent(false);
            var stoppedEvent = new AutoResetEvent(false);
            var newTextEvent = new AutoResetEvent(false);
            manager.Started += (s, e) =>
            {
                startedEvent.Set();
                //Assert.Equal(manager.Converter, e.Converter);
                //Assert.Equal(manager.Recorder, e.Recorder);
                Assert.Null(e.Data);
                Assert.False(e.IsHandled);
                Assert.Null(e.Text);
            };
            manager.Stopped += (s, e) =>
            {
                stoppedEvent.Set();
                //Assert.Equal(manager.Converter, e.Converter);
                //Assert.Equal(manager.Recorder, e.Recorder);
                Assert.Equal(manager.Data, e.Data);
                Assert.False(e.IsHandled);
                Assert.Null(e.Text);
            };
            manager.NewText += (s, e) =>
            {
                newTextEvent.Set();
                //Assert.Equal(manager.Converter, e.Converter);
                //Assert.Equal(manager.Recorder, e.Recorder);
                Assert.Equal(manager.Data, e.Data);
                Assert.False(e.IsHandled);
                Assert.Equal(manager.Text, e.Text);
            };

            //BaseRecorderTest(manager);

            manager.Change();
            Thread.Sleep(2000);
            manager.Change();

            manager.Start();
            Thread.Sleep(2000);
            manager.Stop();

            Assert.True(startedEvent.WaitOne(TimeSpan.FromSeconds(10)));
            Assert.True(stoppedEvent.WaitOne(TimeSpan.FromSeconds(10)));
            Assert.True(newTextEvent.WaitOne(TimeSpan.FromSeconds(10)));

            //BaseRecorderTest(manager.Recorder);
            BaseDisposeTest(manager);
        }
    }
}
