using System.IO;
using System.Runtime.InteropServices;
using VoiceActions.NET.Recorders.Core;

namespace VoiceActions.NET.Recorders
{
    public class WinmmRecorder : BaseRecorder, IRecorder
    {
        #region Private methods

        // Winmm.dll is used for recording speech
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int MciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        private static void MciSendString(string command) => MciSendString(command, "", 0, 0);

        #endregion

        #region Public methods

        public new void Start()
        {
            MciSendString("open new Type waveaudio Alias recsound");
            MciSendString("record recsound");

            base.Start();
        }

        public new void Stop()
        {
            var path = Path.GetTempFileName();
            MciSendString("save recsound " + path);
            MciSendString("close recsound ");

            Data = File.ReadAllBytes(path);
            File.Delete(path);

            base.Stop();
        }

        #endregion
    }
}
