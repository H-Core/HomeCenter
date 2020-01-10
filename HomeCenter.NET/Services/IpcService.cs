using System;
using System.Threading;
using System.Threading.Tasks;
using H.Pipes;
using H.Pipes.Args;

namespace HomeCenter.NET.Services
{
    public sealed class IpcService : IDisposable, IAsyncDisposable
    {
        #region Properties

        private ExceptionService ExceptionService { get; }
        private RunnerService RunnerService { get; }

        private PipeServer<string> MainApplicationServer { get; } = new PipeServer<string>("H.MainApplication");

        #endregion

        #region Constructors

        public IpcService(ExceptionService exceptionService, RunnerService runnerService)
        {
            ExceptionService = exceptionService ?? throw new ArgumentNullException(nameof(exceptionService));
            RunnerService = runnerService ?? throw new ArgumentNullException(nameof(runnerService));

            MainApplicationServer.MessageReceived += MainApplicationServer_OnMessageReceived;
        }

        #endregion

        #region Event Handlers

        private async void MainApplicationServer_OnMessageReceived(object? sender, ConnectionMessageEventArgs<string> args)
        {
            try
            {
                await RunnerService.RunAsync(args.Message).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await ExceptionService.HandleExceptionAsync(exception).ConfigureAwait(false);
            }
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
                await ExceptionService.HandleExceptionAsync(exception, cancellationToken).ConfigureAwait(false);
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
                await ExceptionService.HandleExceptionAsync(exception, cancellationToken).ConfigureAwait(false);
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
