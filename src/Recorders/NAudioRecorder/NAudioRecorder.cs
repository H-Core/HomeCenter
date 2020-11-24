using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using H.Core.Recorders;
using NAudio.Wave;

namespace H.Recorders
{
    public class NAudioRecorder : Recorder
    {
        #region Properties

        public int Rate { get; set; } = 8000;
        public int Bits { get; set; } = 16;
        public int Channels { get; set; } = 1;

        private IWaveIn? WaveIn { get; set; }
        private MemoryStream? Stream { get; set; }
        private WaveFileWriter? WaveFileWriter { get; set; }

        #endregion

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
            if (IsInitialized)
            {
                throw new InvalidOperationException("Already initialized");
            }

            WaveIn ??= new WaveInEvent
            {
                WaveFormat = new WaveFormat(Rate, Bits, Channels)
            };

            WaveIn.DataAvailable += (_, args) =>
            {
                if (WaveFileWriter != null)
                {
                    WaveFileWriter.Write(args.Buffer, 0, args.BytesRecorded);
                    WaveFileWriter.Flush();
                }

                RawData ??= Array.Empty<byte>();
                RawData = RawData.Concat(args.Buffer).ToArray();

                OnRawDataReceived(args.Buffer);
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

            IsInitialized = true;

            return Task.CompletedTask;
        }

        // ReSharper disable AccessToDisposedClosure
        public override async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (WaveIn == null || 
                !IsInitialized)
            {
                throw new InvalidOperationException("Is not initialized");
            }

            WaveFileWriter?.Dispose();
            Stream?.Dispose();

            Stream = new MemoryStream();
            WaveFileWriter = new WaveFileWriter(Stream, WaveIn.WaveFormat);

            WaveIn.StartRecording();

            await base.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public override async Task StopAsync(CancellationToken cancellationToken = default)
        {
            WaveIn?.StopRecording();

            if (Stream != null)
            {
                Stream.Position = 0;

                WavData = Stream.ToArray();
            }

            await base.StopAsync(cancellationToken).ConfigureAwait(false);
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

            WaveFileWriter?.Dispose();
            WaveFileWriter = null;

            Stream?.Dispose();
            Stream = null;

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
