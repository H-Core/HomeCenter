using System;
using System.Timers;
using VoiceActions.NET.Recorders.Core;

namespace VoiceActions.NET.Recorders
{
    public class AutoStopRecorder : BaseRecorder, IAutoStopRecorder
    {
        #region Properties

        public IRecorder Recorder { get; private set; }
        public double Interval { get; set; }

        private bool _autoStopEnabled = true;
        public bool AutoStopEnabled
        {
            get => _autoStopEnabled;
            set
            {
                _autoStopEnabled = value;
                if (!value)
                {
                    StopTimer();
                }
            }
        }

        private Timer Timer { get; set; }

        #endregion

        #region Constructors

        public AutoStopRecorder(IRecorder recorder, int interval)
        {
            Recorder = recorder;

            Interval = interval;
        }

        #endregion

        #region Public methods

        public override void Start()
        {
            if (AutoStopEnabled)
            {
                StartTimer();
            }

            Recorder.Start();
            base.Start();
        }

        public override void Stop()
        {
            StopTimer();

            Recorder.Stop();
            Data = Recorder.Data;
            base.Stop();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Timer?.Dispose();
            Timer = null;

            Recorder?.Dispose();
            Recorder = null;
        }

        #endregion

        #region Event Handlers

        private void OnTimerOnElapsed(object sender, ElapsedEventArgs args) => Stop();

        #endregion

        #region Private methods

        private void StartTimer()
        {
            Timer = new Timer(Interval);
            Timer.Elapsed += OnTimerOnElapsed;
            Timer.Start();
        }

        private void StopTimer()
        {
            Timer?.Stop();
            Timer?.Dispose();
            Timer = null;
        }

        #endregion
    }
}
