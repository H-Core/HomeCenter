using System;
using System.Threading;
using System.Threading.Tasks;
using H.Pipes;

namespace H.NET.SearchDeskBand
{
    public sealed class IpcService : IAsyncDisposable
    {
        #region Properties

        private PipeClient<string> PipeClient { get; } = new PipeClient<string>("H.Control");

        #endregion

        #region Events

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<string> MessageReceived;
        public event EventHandler<Exception> ExceptionOccurred;

        private void OnConnected()
        {
            Connected?.Invoke(null, EventArgs.Empty);
        }

        private void OnDisconnected()
        {
            Disconnected?.Invoke(null, EventArgs.Empty);
        }

        private void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(null, message);
        }

        private void OnExceptionOccurred(Exception exception)
        {
            ExceptionOccurred?.Invoke(null, exception);
        }

        #endregion

        #region Constructors

        public IpcService()
        {
            PipeClient.Connected += (sender, args) => OnConnected();
            PipeClient.Disconnected += (sender, args) => OnDisconnected();
            PipeClient.MessageReceived += (sender, args) => OnMessageReceived(args.Message);
            PipeClient.ExceptionOccurred += (sender, args) => OnExceptionOccurred(args.Exception);
        }

        #endregion

        #region Public methods

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            await PipeClient.ConnectAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task WriteAsync(string message, CancellationToken cancellationToken = default)
        {
            await PipeClient.WriteAsync(message, cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await PipeClient.DisposeAsync().ConfigureAwait(false);
        }

        #endregion
    }
}
