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
        private PushStreamContent PushStreamContent { get; }
        private Task<HttpResponseMessage> PostTask { get; }

        private ConcurrentQueue<byte[]> WriteQueue { get; } = new ConcurrentQueue<byte[]>();
        private bool IsStopped { get; set; }

        #endregion

        #region Constructors

        internal WitAiStreamingRecognition(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));

            HttpClient = new HttpClient();

            HttpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + Token);

            PushStreamContent = new PushStreamContent(async (stream, httpContent, transportContext) =>
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
            })
            {
                Headers =
                {
                    ContentType = MediaTypeHeaderValue.Parse("audio/wav")
                }
            };
            
            PostTask = HttpClient.PostAsync("https://api.wit.ai/speech", PushStreamContent);
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

            var message = await PostTask.ConfigureAwait(false);
            
            message.EnsureSuccessStatusCode();

            var json = await message.Content.ReadAsStringAsync();

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
                PushStreamContent.Dispose();
                PostTask.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
