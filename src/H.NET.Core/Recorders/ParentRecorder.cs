using System.Threading;
using System.Threading.Tasks;

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

        public override async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (Recorder == null || Recorder.IsInitialized)
            {
                return;
            }

            await Recorder.InitializeAsync(cancellationToken);
        }

        public override async Task StartAsync(CancellationToken cancellationToken = default)
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

            if (!Recorder.IsInitialized)
            {
                await Recorder.InitializeAsync(cancellationToken);
            }

            await Recorder.StartAsync(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken = default)
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

            await Recorder.StopAsync(cancellationToken);

            RawData = Recorder.RawData;
            WavData = Recorder.WavData;

            await base.StopAsync(cancellationToken);
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
