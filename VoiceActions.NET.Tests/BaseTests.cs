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

            BaseRecorderTest(manager.Recorder);
            BaseRecorderTest(manager);

            BaseDisposeTest(manager);
        }
    }
}
