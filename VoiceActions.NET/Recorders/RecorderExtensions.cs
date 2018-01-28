using System.Threading.Tasks;

namespace VoiceActions.NET.Recorders
{
    public static class RecorderExtensions
    {
        public static async void StartWithTimeout(this IRecorder recorder, int millisecondsTimeout)
        {
            recorder.Start();

            await Task.Delay(millisecondsTimeout);

            recorder.Stop();
        }

        public static void ChangeWithTimeout(this IRecorder recorder, int millisecondsTimeout)
        {
            if (!recorder.IsStarted)
            {
                recorder.StartWithTimeout(millisecondsTimeout);
            }
            else
            {
                recorder.Stop();
            }
        }
    }
}
