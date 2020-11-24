using System;
using System.Threading;
using H.Core;
using H.Core.Attributes;
using H.Core.Recorders;

namespace H.Recorders
{
    [AllowMultipleInstance(false)]
    public class StreamEmulatorRecorder : ParentRecorder
    {
        #region Properties

        public int Interval { get; }
        public Timer Timer { get; }

        #endregion

        #region Events

        public event EventHandler<VoiceActionsEventArgs>? NewPartialData;
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

        private async void OnTimer(object sender)
        {
            if (!IsStarted)
            {
                return;
            } 

            await StopAsync();
            OnNewPartialData();
            //File.WriteAllBytes($"D:/voice_{new Random().Next()}.wav", Data);
            await StartAsync();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Timer.Dispose();

            base.Dispose();
        }

        #endregion
    }
}
