using System.Timers;
using VoiceActions.NET.Recorders.Core;

namespace VoiceActions.NET.Recorders
{
    public class AutoStopRecorder : BaseRecorder, IAutoStopRecorder
    {
        #region Properties

        public Timer Timer { get; private set; } = new Timer();
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

        public IRecorder Recorder { get; private set; }

        #endregion

        #region Constructors

        public AutoStopRecorder(IRecorder recorder, int interval)
        {
            Recorder = recorder;

            Timer.Interval = interval;
            Timer.Elapsed += OnTimerOnElapsed;
        }

        #endregion

        #region Public methods

        public override void Start()
        {
            Timer.Start();
            Recorder.Start();
            base.Start();
        }

        public override void Stop()
        {
            Timer.Stop();
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
    }
}
