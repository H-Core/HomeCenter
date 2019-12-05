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
        public WaveInEvent WaveIn { get; set; }
        public MemoryStream Stream { get; set; }
        public WaveFileWriter WaveFile { get; set; }

        public event EventHandler<WaveInEventArgs> NewData;

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

                Data = Data ?? Array.Empty<byte>();
                Data = Data.Concat(args.Buffer).ToArray();

                NewData?.Invoke(this, args);
            };
        }

        #endregion

        #region Public methods

        public override void Start()
        {
            WaveIn.StartRecording();

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
