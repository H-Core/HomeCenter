using System;
using System.Threading;
using System.Threading.Tasks;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Recorders;
using VoiceActions.NET.Tests.Utilities;
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

        protected void BaseRecorderTest(IRecorder recorder, PlatformID? platformId = null, int timeout = 1000)
        {
            Assert.NotNull(recorder);
            var autoStopRecorder = recorder as AutoStopRecorder;
            if (autoStopRecorder != null)
            {
                Assert.False(autoStopRecorder.AutoStopEnabled);
                Assert.InRange(autoStopRecorder.Interval, 1, int.MaxValue);
            }

            if (!CheckPlatform(platformId))
            {
                Output?.WriteLine($"Recorder: {recorder} not support current system: {Environment.OSVersion}");
                return;
            }

            recorder.Start();
            Thread.Sleep(timeout);
            recorder.Stop();

            Assert.NotNull(recorder.Data);
            Assert.InRange(recorder.Data.Length, 1, int.MaxValue);

            BaseDisposeTest(recorder);
            if (autoStopRecorder != null)
            {
                Assert.Null(autoStopRecorder.Recorder);
            }

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

        protected static void BaseArgsTest(VoiceManager manager, VoiceActionsEventArgs args)
        {
            Assert.Equal(manager.Converter, args.Converter);
            Assert.Equal(manager.Recorder, args.Recorder);
            Assert.Equal(manager.Data, args.Data);
            Assert.Equal(manager.Text, args.Text);
        }

        protected void BaseVoiceManagerTest(VoiceManager manager, PlatformID? platformId = null, int timeout = 1000, int waitEventTimeout = 20000)
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
            var actionEvent = new AutoResetEvent(false);
            manager.Started += (s, e) =>
            {
                startedEvent.Set();
                BaseArgsTest(manager, e);
            };
            manager.Stopped += (s, e) =>
            {
                stoppedEvent.Set();
                BaseArgsTest(manager, e);
            };
            manager.NewText += (s, e) =>
            {
                newTextEvent.Set();
                BaseArgsTest(manager, e);

                if (string.Equals(manager.Text, "проверка", StringComparison.OrdinalIgnoreCase))
                {
                    actionEvent.Set();
                }
            };

            manager.Change();
            Thread.Sleep(timeout);
            manager.Change();

            manager.Start();
            Thread.Sleep(timeout);
            manager.Stop();

            manager.StartWithoutAutostop();
            Thread.Sleep(timeout);
            manager.Stop();

            manager.ProcessSpeech(TestUtilities.GetRawSpeech("speech1.wav"));

            Assert.True(startedEvent.WaitOne(TimeSpan.FromMilliseconds(waitEventTimeout)));
            Assert.True(stoppedEvent.WaitOne(TimeSpan.FromMilliseconds(waitEventTimeout)));
            Assert.True(newTextEvent.WaitOne(TimeSpan.FromMilliseconds(waitEventTimeout)));
            Assert.True(actionEvent.WaitOne(TimeSpan.FromMilliseconds(waitEventTimeout)));

            BaseRecorderTest(manager);

            Assert.Null(manager.Recorder);
            Assert.Null(manager.Converter);
        }
    }
}
