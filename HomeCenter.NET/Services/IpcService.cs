using System;
using System.Threading;
using System.Threading.Tasks;
using HomeCenter.NET.Properties;
using H.Pipes;

namespace HomeCenter.NET.Services
{
    public sealed class IpcService : IDisposable
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
                await MainApplicationServer.StartAsync(cancellationToken: cancellationToken);
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
                await MainApplicationServer.WriteAsync(command, cancellationToken: cancellationToken);
            }
            catch (Exception exception)
            {
                RunnerService.Run($"print {exception.Message}");
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
        }

        #endregion
    }
}
