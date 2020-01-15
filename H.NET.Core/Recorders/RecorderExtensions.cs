using System.Threading;
using System.Threading.Tasks;

namespace H.NET.Core.Recorders
{
    public static class RecorderExtensions
    {
        public static async Task StartWithTimeoutAsync(this IRecorder recorder, int millisecondsTimeout, CancellationToken cancellationToken = default)
        {
            await recorder.StartAsync(cancellationToken);

            await Task.Delay(millisecondsTimeout, cancellationToken);

            await recorder.StopAsync(cancellationToken);
        }

        public static async Task ChangeWithTimeoutAsync(this IRecorder recorder, int millisecondsTimeout, CancellationToken cancellationToken = default)
        {
            if (!recorder.IsStarted)
            {
                await recorder.StartWithTimeoutAsync(millisecondsTimeout, cancellationToken);
            }
            else
            {
                await recorder.StopAsync(cancellationToken);
            }
        }
    }
}
