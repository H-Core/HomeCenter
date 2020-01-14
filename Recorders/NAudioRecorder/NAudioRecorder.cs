using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core.Recorders;
using NAudio.Wave;

namespace H.NET.Recorders
{
    public class NAudioRecorder : Recorder
    {
        public int Rate { get; set; } = 8000;
        public int Bits { get; set; } = 16;
        public int Channels { get; set; } = 1;

        private WaveInEvent? WaveIn { get; set; }
        private MemoryStream? Stream { get; set; }
        private WaveFileWriter? WaveFile { get; set; }


        #region Constructors

        public NAudioRecorder()
        {
            AddSetting(nameof(Rate), o => Rate = o, NotNegative, 8000);
            AddSetting(nameof(Bits), o => Bits = o, NotNegative, 16);
            AddSetting(nameof(Channels), o => Channels = o, NotNegative, 1);
        }

        #endregion

        #region Public methods

        // ReSharper disable AccessToDisposedClosure
        public override Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            WaveIn ??= new WaveInEvent
            {
                WaveFormat = new WaveFormat(Rate, Bits, Channels)
            };

            WaveIn.DataAvailable += (sender, args) =>
            {
                if (WaveFile != null)
                {
                    WaveFile.Write(args.Buffer, 0, args.BytesRecorded);
                    WaveFile.Flush();
                }

                RawData ??= Array.Empty<byte>();
                RawData = RawData.Concat(args.Buffer).ToArray();

                OnNewRawData(args.Buffer);
            };

            using (var stream = new MemoryStream())
            {
                using var writer = new BinaryWriter(stream, Encoding.UTF8);

                // Fake Wav header of current format
                writer.Write(Encoding.UTF8.GetBytes("RIFF"));
                writer.Write(int.MaxValue);
                writer.Write(Encoding.UTF8.GetBytes("WAVE"));

                writer.Write(Encoding.UTF8.GetBytes("fmt "));
                WaveIn.WaveFormat.Serialize(writer);

                writer.Write(Encoding.UTF8.GetBytes("data"));
                writer.Write(int.MaxValue);

                stream.Position = 0;
                WavHeader = stream.ToArray();
            }

            return Task.CompletedTask;
        }

        // ReSharper disable AccessToDisposedClosure
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
        public string? Name { get; set; }
        public int Channels { get; set; }
    }
}
