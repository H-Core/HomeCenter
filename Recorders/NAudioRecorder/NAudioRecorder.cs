using System.Linq;
using H.NET.Core.Recorders;
using NAudio.Wave;

namespace H.NET.Recorders
{
    public class NAudioRecorder : Recorder
    {
        private WaveInEvent WaveIn { get; set; }

        #region Constructors

        public NAudioRecorder()
        {
            WaveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(44100, 1)
            };

            WaveIn.DataAvailable += (sender, args) =>
            {
                Data = Data ?? new byte[0];

                Data = Data.Concat(args.Buffer).ToArray();
            };
        }

        #endregion

        #region Public methods

        public override void Start()
        {
            WaveIn.StartRecording();

            base.Start();
        }

        public override void Stop()
        {
            WaveIn.StopRecording();

            /*
            using (var waveFile = new WaveFileWriter(@"D:\Test0001.wav", WaveIn.WaveFormat))
            {
                waveFile.Write(Data, 0, Data.Length);
                waveFile.Flush();
            }
            */

            base.Stop();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            WaveIn?.Dispose();
            WaveIn = null;
        }

        #endregion
    }
}
