using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H.NET.Core.Recorders;
using NAudio.Wave;

namespace H.NET.Recorders
{
    public class NAudioRecorder : Recorder
    {
        public int Rate { get; }
        public int Bits { get; }
        public int Channels { get; }

        public WaveInEvent WaveIn { get; set; }
        public MemoryStream Stream { get; set; }
        public WaveFileWriter WaveFile { get; set; }


        #region Constructors

        public NAudioRecorder(int rate = 8000, int bits = 16, int channels = 1)
        {
            Rate = rate;
            Bits = bits;
            Channels = channels;
            WaveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(rate, bits, channels)
            };

            WaveIn.DataAvailable += (sender, args) =>
            {
                if (WaveFile != null)
                {
                    WaveFile.Write(args.Buffer, 0, args.BytesRecorded);
                    WaveFile.Flush();
                }

                RawData = RawData ?? Array.Empty<byte>();
                RawData = RawData.Concat(args.Buffer).ToArray();

                OnNewRawData(args.Buffer);
            };
        }

        #endregion

        #region Public methods

        public override void Start()
        {
            WaveFile?.Dispose();

            Stream = new MemoryStream();
            WaveFile = new WaveFileWriter(Stream, WaveIn.WaveFormat);

            WaveIn.StartRecording();

            base.Start();
        }

        public override void Stop()
        {
            WaveIn.StopRecording();

            Stream.Position = 0;

            WavData = Stream.ToArray();

            Stream.Position = 0;

            base.Stop();
        }

        public static List<DeviceInfo> GetAvailableDevices()
        {
            return Enumerable
                .Range(0, WaveInEvent.DeviceCount)
                .Select(WaveInEvent.GetCapabilities)
                .Select(capability => new DeviceInfo
                {
                    Name = capability.ProductName,
                    Channels = capability.Channels,
                })
                .ToList();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            WaveFile?.Dispose();
            WaveFile = null;

            WaveIn?.Dispose();
            WaveIn = null;
        }

        #endregion
    }

    public class DeviceInfo
    {
        public string Name { get; set; }
        public int Channels { get; set; }
    }
}
