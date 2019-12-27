using System;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core.Utilities;

namespace H.NET.Core.Converters
{
    public abstract class StreamingRecognition : DisposableObject, IStreamingRecognition
    {
        #region Events

        public event EventHandler<VoiceActionsEventArgs> AfterPartialResults;
        public event EventHandler<VoiceActionsEventArgs> AfterFinalResults;

        protected void OnAfterPartialResults(string value)
        {
            AfterPartialResults?.Invoke(this, new VoiceActionsEventArgs
            {
                Text = value,
            });
        }

        protected void OnAfterFinalResults(string value)
        {
            AfterFinalResults?.Invoke(this, new VoiceActionsEventArgs
            {
                Text = value,
            });
        }

        #endregion

        #region Public methods

        public abstract Task WriteAsync(byte[] bytes, CancellationToken cancellationToken = default);
        public abstract Task StopAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
