namespace H.NET.Core.Recorders
{
    public class ParentRecorder : Recorder
    {
        #region Properties

        public IRecorder Recorder { get; protected set; }

        #endregion

        #region Events

        protected override VoiceActionsEventArgs CreateArgs() =>
            new VoiceActionsEventArgs { Recorder = Recorder, Data = Data };

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
            Data = Recorder.Data;
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
