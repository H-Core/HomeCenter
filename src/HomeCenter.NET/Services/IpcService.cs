using System;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using H.Pipes;
using H.Pipes.AccessControl;
using H.Pipes.Args;

namespace HomeCenter.NET.Services
{
    public sealed class IpcService : IDisposable, IAsyncDisposable
    {
        #region Properties

        private ExceptionService ExceptionService { get; }
        private RunnerService RunnerService { get; }

        private PipeServer<string> PipeServer { get; } = new PipeServer<string>("H.Control");

        #endregion

        #region Constructors

        public IpcService(ExceptionService exceptionService, RunnerService runnerService)
        {
            ExceptionService = exceptionService ?? throw new ArgumentNullException(nameof(exceptionService));
            RunnerService = runnerService ?? throw new ArgumentNullException(nameof(runnerService));

            PipeServer.AddAccessRules(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
            PipeServer.MessageReceived += PipeServer_OnMessageReceived;
            PipeServer.ExceptionOccurred += PipeServer_OnExceptionOccurred;
            PipeServer.ClientConnected += PipeServer_OnClientConnected;
            PipeServer.ClientDisconnected += PipeServer_OnClientDisconnected;
        }

        #endregion

        #region Event Handlers

        private async void PipeServer_OnClientConnected(object? sender, ConnectionEventArgs<string> e)
        {
            try
            {
                await RunnerService.RunAsync("print PipeServer: Client connected").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await ExceptionService.HandleExceptionAsync(exception).ConfigureAwait(false);
            }
        }

        private async void PipeServer_OnClientDisconnected(object? sender, ConnectionEventArgs<string> e)
        {
            try
            {
                await RunnerService.RunAsync("print PipeServer: Client disconnected").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await ExceptionService.HandleExceptionAsync(exception).ConfigureAwait(false);
            }
        }

        private async void PipeServer_OnMessageReceived(object? sender, ConnectionMessageEventArgs<string> args)
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

        private async void PipeServer_OnExceptionOccurred(object? sender, ExceptionEventArgs args)
        {
            await ExceptionService.HandleExceptionAsync(args.Exception).ConfigureAwait(false);
        }

        #endregion

        #region Public methods

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await PipeServer.StartAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
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
                await PipeServer.WriteAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);
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
            await PipeServer.DisposeAsync().ConfigureAwait(false);
        }

        #endregion
    }
}
