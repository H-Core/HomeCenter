namespace VoiceActions.NET.Recorders.Core
{
    public class ParentRecorder : BaseRecorder
    {
        #region Properties

        public IRecorder Recorder { get; protected set; }

        #endregion

        #region Constructors

        public ParentRecorder()
        {
        }

        public ParentRecorder(IRecorder recorder)
        {
            Recorder = recorder;
        }

        #endregion

        #region Public methods

        public override void Start()
        {
            Recorder.Start();
            base.Start();
        }

        public override void Stop()
        {
            Recorder.Stop();
            Data = Recorder.Data;
            base.Stop();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Recorder?.Dispose();
            Recorder = null;
        }

        #endregion
    }
}
