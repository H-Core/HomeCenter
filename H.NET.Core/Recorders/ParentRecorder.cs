using System;

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
            var recorder = Recorder ?? throw new Exception("Recorder is null");

            recorder.Start();
            base.Start();
        }

        public override void Stop()
        {
            var recorder = Recorder ?? throw new Exception("Recorder is null");

            recorder.Stop();
            Data = recorder.Data;
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
