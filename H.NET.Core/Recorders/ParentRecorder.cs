namespace H.NET.Core.Recorders
{
    public class ParentRecorder : Recorder
    {
        #region Properties

        public IRecorder Recorder { get; protected set; }

        #endregion

        #region Constructors

        #endregion

        #region Public methods

        public override void Start()
        {
            if (IsStarted)
            {
                return;
            }

            if (Recorder == null)
            {
                Log("Recorder is not found");
                return;
            }

            Recorder.Start();
            base.Start();
        }

        public override void Stop()
        {
            if (!IsStarted)
            {
                return;
            }

            if (Recorder == null)
            {
                Log("Recorder is not found");
                return;
            }

            Recorder.Stop();
            RawData = Recorder.RawData;
            WavData = Recorder.WavData;
            base.Stop();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            Recorder?.Dispose();
            Recorder = null;
        }

        #endregion
    }
}
