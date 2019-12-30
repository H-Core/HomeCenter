using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.NET.Core.Converters
{
    public abstract class Converter : Module, IConverter
    {
        #region Properties

        public bool IsStreamingRecognitionSupported => false;

        #endregion

        #region Methods

        public abstract Task<string> ConvertAsync(byte[] bytes, CancellationToken cancellationToken = default);

        public virtual Task<IStreamingRecognition> StartStreamingRecognitionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected async Task<string> ConvertOverStreamingRecognition(byte[] bytes, CancellationToken cancellationToken = default)
        {
            using var recognition = await StartStreamingRecognitionAsync(cancellationToken);
            var response = string.Empty;
            recognition.AfterFinalResults += (sender, args) => response = args.Text;

            await recognition.WriteAsync(bytes, cancellationToken);
            await recognition.StopAsync(cancellationToken);

            return response;
        }

        #endregion
    }
}
