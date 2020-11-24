using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.NET.Core.Recorders
{
    public static class RecorderExtensions
    {
        public static async Task StartWithTimeoutAsync(this IRecorder recorder, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            if (!recorder.IsInitialized)
            {
                await recorder.InitializeAsync(cancellationToken);
            }

            await recorder.StartAsync(cancellationToken);

            await Task.Delay(timeout, cancellationToken);

            await recorder.StopAsync(cancellationToken);
        }

        public static async Task ChangeWithTimeoutAsync(this IRecorder recorder, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            if (!recorder.IsInitialized)
            {
                await recorder.InitializeAsync(cancellationToken);
            }

            if (!recorder.IsStarted)
            {
                await recorder.StartWithTimeoutAsync(timeout, cancellationToken);
            }
            else
            {
                await recorder.StopAsync(cancellationToken);
            }
        }
    }
}
