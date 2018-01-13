using System.Timers;
using VoiceActions.NET.Recorders.Core;

namespace VoiceActions.NET.Recorders
{
    public class AutoStopRecorder<T> : BaseRecorder, IAutoStopRecorder where T : IRecorder, new()
    {
        #region Properties

        private Timer Timer { get; set; } = new Timer();
        public double Interval {  get => Timer.Interval; set => Timer.Interval = value; }
        public bool AutoStopEnabled
        {
            get => Timer.Enabled;
            set
            {
                Timer.Stop();
                Timer.Enabled = value;
            }
        }

        public IRecorder Recorder { get; private set; } = new T();

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
            Recorder.Start();
            base.Start();
        }

        public new void Stop()
        {
            Timer.Stop();
            Recorder.Stop();
            Data = Recorder.Data;
            base.Stop();
        }

        #endregion

        #region IDisposable

        public new void Dispose()
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
    }
}
