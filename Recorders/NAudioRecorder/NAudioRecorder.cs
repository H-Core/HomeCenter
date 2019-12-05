using System.IO;
using System.Linq;
using H.NET.Core.Recorders;
using NAudio.Wave;

namespace H.NET.Recorders
{
    public class NAudioRecorder : Recorder
    {
        private WaveInEvent WaveIn { get; set; }
        public MemoryStream Stream { get; set; }
        public WaveFileWriter WaveFile { get; set; }

        #region Constructors

        public NAudioRecorder()
        {
            WaveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(44100, 1)
            };

            WaveIn.DataAvailable += (sender, args) =>
            {
                WaveFile.Write(args.Buffer, 0, args.BytesRecorded);
                WaveFile.Flush();

                Data = Data ?? new byte[0];
                Data = Data.Concat(args.Buffer).ToArray();
            };
        }

        #endregion

        #region Public methods

        public override void Start()
        {
            WaveIn.StartRecording();

            Stream?.Dispose();
            WaveFile?.Dispose();

            Stream = new MemoryStream();
            WaveFile = new WaveFileWriter(Stream, WaveIn.WaveFormat);

            base.Start();
        }

        public override void Stop()
        {
            WaveIn.StopRecording();

            Stream.Position = 0;

            Data = Stream.ToArray();

            Stream.Position = 0;

            base.Stop();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            Stream?.Dispose();
            Stream = null;

            WaveFile?.Dispose();
            WaveFile = null;

            WaveIn?.Dispose();
            WaveIn = null;
        }

        #endregion
    }
}
