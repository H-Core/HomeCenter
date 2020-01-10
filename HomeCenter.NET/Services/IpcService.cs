using System;
using System.Threading;
using System.Threading.Tasks;
using HomeCenter.NET.Properties;
using H.Pipes;

namespace HomeCenter.NET.Services
{
    public sealed class IpcService : IDisposable, IAsyncDisposable
    {
        #region Properties

        public RunnerService RunnerService { get; }
        public Settings Settings { get; }

        private PipeServer<string> MainApplicationServer { get; } = new PipeServer<string>("H.MainApplication");

        #endregion

        #region Constructors

        public IpcService(RunnerService runnerService, Settings settings)
        {
            RunnerService = runnerService ?? throw new ArgumentNullException(nameof(runnerService));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));

            MainApplicationServer.MessageReceived += (sender, args) => RunnerService.Run(args.Message);
        }

        #endregion

        #region Public methods

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await MainApplicationServer.StartAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                RunnerService.Run($"print {exception.Message}");
            }
        }

        public async Task SendToProcessesAsync(string command, CancellationToken cancellationToken = default)
        {
            try
            {
                await MainApplicationServer.WriteAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                RunnerService.Run($"print {exception.Message}");
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
        }

        public async ValueTask DisposeAsync()
        {
            await MainApplicationServer.DisposeAsync().ConfigureAwait(false);
        }

        #endregion
    }
}
