using System;
using System.Collections.Concurrent;
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
    public sealed class YandexStreamingRecognition : StreamingRecognition
    {
        #region Properties

        private AsyncDuplexStreamingCall<StreamingRecognitionRequest, StreamingRecognitionResponse> Call { get; }

        private ConcurrentQueue<byte[]> WriteQueue { get; } = new ConcurrentQueue<byte[]>();
        private Task ReceiveTask { get; }
        private Task WriteTask { get; }
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
            WriteTask = Task.Run(async () =>
            {
                while (!IsFinished)
                {
                    if (!WriteQueue.TryDequeue(out var bytes))
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1));
                        continue;
                    }

                    await Call.RequestStream.WriteAsync(new StreamingRecognitionRequest
                    {
                        AudioContent = ByteString.CopyFrom(bytes, 0, bytes.Length),
                    }).ConfigureAwait(false);
                }
            });
        }

        #endregion

        #region Public methods

        public override Task WriteAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            WriteQueue.Enqueue(bytes);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await WriteTask;

            await Call.RequestStream.CompleteAsync().ConfigureAwait(false);

            await ReceiveTask;
        }

        public void Dispose()
        {
            ReceiveTask?.Dispose();
            WriteTask?.Dispose();
            Call?.Dispose();
        }

        #endregion
    }
}
