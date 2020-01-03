using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core.Converters;
using Newtonsoft.Json;

#nullable enable

namespace H.NET.Converters
{
    public sealed class WitAiStreamingRecognition : StreamingRecognition
    {
        #region Properties

        private string Token { get; }

        private HttpClient HttpClient { get; }
        private HttpRequestMessage HttpRequestMessage { get; }
        private Task<HttpResponseMessage> SendTask { get; }

        private ConcurrentQueue<byte[]> WriteQueue { get; } = new ConcurrentQueue<byte[]>();
        private bool IsStopped { get; set; }
        private bool IsFinished { get; set; }

        #endregion

        #region Constructors

        internal WitAiStreamingRecognition(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));

            HttpClient = new HttpClient();
            HttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.wit.ai/speech?v=20200103")
            {
                Headers =
                {
                    { "Authorization", $"Bearer {Token}" },
                    { "Transfer-encoding", "chunked" },
                },
                Content = new PushStreamContent(async (stream, httpContent, transportContext) =>
                {
                    {
                        using var writer = new BinaryWriter(stream);
                        
                        while (!IsStopped || !WriteQueue.IsEmpty)
                        {
                            // TODO: Combine all accumulated data in the queue into one message
                            if (!WriteQueue.TryDequeue(out var bytes))
                            {
                                await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
                                continue;
                            }

                            writer.Write(bytes);
                        }

                        stream.Flush();
                    }

                    IsFinished = true;
                }, MediaTypeHeaderValue.Parse("audio/wav")),
            };

            SendTask = HttpClient.SendAsync(HttpRequestMessage);
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
            IsStopped = true;

            while (!IsFinished)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
            }

            var response = await SendTask.ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Invalid response: {json}");
            }


            var obj = JsonConvert.DeserializeObject<WitAiResponse>(json);

            OnAfterFinalResults(obj.Text);
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                HttpClient.Dispose();
                HttpRequestMessage.Dispose();
                SendTask.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
