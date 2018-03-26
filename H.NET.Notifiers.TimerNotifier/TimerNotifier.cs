using System.Timers;
using H.NET.Core.Notifiers;

namespace H.NET.Notifiers
{
    public class TimerNotifier : Notifier
    {
        #region Properties

        private int _interval;
        public int Interval
        {
            get => _interval;
            set
            {
                _interval = value;

                if (value <= 0)
                {
                    return;
                }

                Timer?.Dispose();
                Timer = new Timer(value);
                Timer.Elapsed += OnElapsed;
                Timer.Start();
            }
        }

        public int Frequency { get; set; }

        private Timer Timer { get; set; }
        private int CurrentTime { get; set; } = int.MaxValue;

        #endregion

        #region Events

        #endregion

        #region Constructors

        public TimerNotifier()
        {
            AddSetting("Interval", o => Interval = o, value => value > 0, int.MaxValue);
            AddSetting("Frequency", o => Frequency = o, value => value >= 0, 0);
        }

        #endregion

        #region IDisposable
        
        public override void Dispose()
        {
            base.Dispose();

            Timer.Stop();
            Timer?.Dispose();
            Timer = null;
        }

        #endregion

        #region Protected methods

        protected virtual void OnElapsed() => OnEvent();

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            CurrentTime += Interval;
            if (Frequency > 0 && CurrentTime > 0 && CurrentTime < Frequency)
            {
                return;
            }

            CurrentTime = 0;
            OnElapsed();
        } 

        #endregion

    }
}
