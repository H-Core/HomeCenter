using System;
using System.Timers;
using VoiceActions.NET.Recorders.Core;

namespace VoiceActions.NET.Recorders
{
    public class AutoStopRecorder<T> : BaseRecorder, IRecorder, IDisposable where T : IRecorder, new()
    {
        #region Properties

        public Timer Timer { get; set; } = new Timer();
        public double Interval => Timer.Interval;
        public IRecorder SpeechRecorder { get; } = new T();

        #endregion

        #region Constructors

        public AutoStopRecorder(int interval)
        {
            Timer.Interval = interval;
            Timer.Elapsed += OnTimerOnElapsed;
        }

        #endregion

        #region Public methods

        public new void Start()
        {
            Timer.Start();
            SpeechRecorder.Start();
            base.Start();
        }

        public new void Stop()
        {
            Timer.Stop();
            SpeechRecorder.Stop();
            Data = SpeechRecorder.Data;
            base.Stop();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Timer?.Dispose();
            Timer = null;
        }

        #endregion

        #region Event Handlers

        private void OnTimerOnElapsed(object sender, ElapsedEventArgs args) => Stop();

        #endregion
    }
}
