using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using H.NET.Core.Converters;
using Yandex.Cloud.Ai.Stt.V2;

namespace H.NET.Converters
{
    public sealed class YandexStreamingRecognition : StreamingRecognition, IDisposable
    {
        #region Properties

        private AsyncDuplexStreamingCall<StreamingRecognitionRequest, StreamingRecognitionResponse> Call { get; }
        private Task ReceiveTask { get; }
        private bool IsFinished { get; set; }

        #endregion

        #region Constructors

        internal YandexStreamingRecognition(AsyncDuplexStreamingCall<StreamingRecognitionRequest, StreamingRecognitionResponse> call)
        {
            Call = call ?? throw new ArgumentNullException(nameof(call));
            
            ReceiveTask = Task.Run(async () =>
            {
                while (!IsFinished && await Call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    var response = Call.ResponseStream.Current;
                    var chunk = response.Chunks
                        .LastOrDefault();
                    var text = chunk?
                        .Alternatives
                        .OrderBy(i => i.Confidence)
                        .FirstOrDefault()?
                        .Text;

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        if (chunk.Final)
                        {
                            IsFinished = true;
                            OnAfterFinalResults(text);
                        }
                        else
                        {
                            OnAfterPartialResults(text);
                        }
                    }

                    Trace.WriteLine($"{DateTime.Now:h:mm:ss.fff} YandexStreamingRecognition: {response}");
                }
            });
        }

        #endregion

        #region Public methods

        public override async Task WriteAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            await Call.RequestStream.WriteAsync(new StreamingRecognitionRequest
            {
                AudioContent = ByteString.CopyFrom(bytes, 0, bytes.Length),
            }).ConfigureAwait(false);
        }

        public override async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await Call.RequestStream.CompleteAsync().ConfigureAwait(false);

            while (!IsFinished)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken);
            }
        }

        public void Dispose()
        {
            ReceiveTask?.Dispose();
            Call?.Dispose();
        }

        #endregion
    }
}
