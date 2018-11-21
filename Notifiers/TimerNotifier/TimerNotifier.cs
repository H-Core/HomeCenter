using System;
using System.Timers;
using H.NET.Core.Notifiers;

namespace H.NET.Notifiers
{
    public class TimerNotifier : Notifier
    {
        #region Properties

        private int _intervalInMilliseconds;
        public int IntervalInMilliseconds {
            get => _intervalInMilliseconds;
            set {
                _intervalInMilliseconds = value;

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
        public int RequiredCount { get; set; }

        private Timer Timer { get; set; }
        private int CurrentTime { get; set; } = int.MaxValue;
        private int CurrentCount { get; set; }

        #endregion

        #region Constructors

        public TimerNotifier()
        {
            AddSetting(nameof(IntervalInMilliseconds), o => IntervalInMilliseconds = o, Positive, int.MaxValue);
            AddSetting(nameof(Frequency), o => Frequency = o, NotNegative, 0);
            AddSetting(nameof(RequiredCount), o => RequiredCount = o, Positive, 1);
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
            CurrentTime += IntervalInMilliseconds;
            if (Frequency > 0 && CurrentTime > 0 && CurrentTime < Frequency)
            {
                return;
            }

            CurrentTime = 0;

            if (CurrentCount < RequiredCount)
            {
                CurrentCount++;
                return;
            }

            try
            {
                OnElapsed();
            }
            catch (Exception exception)
            {
                Log($"Exception: {exception}");
                Log($"Disabling module: {Name}");

                Disable();
            }
            finally
            {
                CurrentCount = 0;
            }
        }

        #endregion

    }
}
