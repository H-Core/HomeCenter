using System;
using System.Threading;
using System.Threading.Tasks;
using H.Pipes;

namespace H.NET.SearchDeskBand
{
    public class IpcService : IAsyncDisposable
    {
        #region Properties

        private PipeClient<string> PipeClient { get; } = new PipeClient<string>("H.MainApplication");

        #endregion

        #region Events

        public event EventHandler<string> MessageReceived;
        public event EventHandler<Exception> ExceptionOccurred;

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
