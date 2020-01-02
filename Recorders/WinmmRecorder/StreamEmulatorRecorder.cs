using System;
using System.Threading;
using H.NET.Core;
using H.NET.Core.Attributes;
using H.NET.Core.Recorders;

namespace H.NET.Recorders
{
    [AllowMultipleInstance(false)]
    public class StreamEmulatorRecorder : ParentRecorder
    {
        #region Properties

        public int Interval { get; private set; }
        public Timer Timer { get; private set; }

        #endregion

        #region Events

        public event EventHandler<VoiceActionsEventArgs> NewPartialData;
        private void OnNewPartialData() => NewPartialData?.Invoke(this, new VoiceActionsEventArgs
        {
            Data = RawData ?? WavData,
        });

        #endregion

        #region Constructors

        public StreamEmulatorRecorder(IRecorder recorder, int interval)
        {
            Recorder = recorder;
            Interval = interval;
            
            Timer = new Timer(OnTimer, null, 0, Interval);
        }

        #endregion

        #region Event handlers

        private void OnTimer(object sender)
        {
            if (!IsStarted)
            {
                return;
            } 

            Stop();
            OnNewPartialData();
            //File.WriteAllBytes($"D:/voice_{new Random().Next()}.wav", Data);
            Start();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Timer?.Dispose();
            Timer = null;

            base.Dispose();
        }

        #endregion
    }
}
