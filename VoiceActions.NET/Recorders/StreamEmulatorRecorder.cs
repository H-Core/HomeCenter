using System;
using System.IO;
using System.Threading;
using VoiceActions.NET.Managers;
using VoiceActions.NET.Recorders.Core;

namespace VoiceActions.NET.Recorders
{
    public class StreamEmulatorRecorder : ParentRecorder
    {
        #region Properties

        public int Interval { get; private set; }
        public Timer Timer { get; private set; }

        #endregion

        #region Events

        public event EventHandler<VoiceActionsEventArgs> NewPartialData;
        private void OnNewPartialData() => NewPartialData?.Invoke(this, CreateArgs());

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
            File.WriteAllBytes($"D:/voice_{new Random().Next()}.wav", Data);
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
