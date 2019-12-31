using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core.Converters;
using Newtonsoft.Json;

#nullable enable

namespace H.NET.Converters
{
    public sealed class WitAiStreamingRecognitionWebRequest : StreamingRecognition
    {
        #region Properties

        private string Token { get; }

        private HttpWebRequest HttpWebRequest { get; }
        private Stream Stream { get; }
        private Task WriteTask { get; }

        private ConcurrentQueue<byte[]> WriteQueue { get; } = new ConcurrentQueue<byte[]>();
        private bool IsStopped { get; set; }

        #endregion

        #region Constructors

        internal WitAiStreamingRecognitionWebRequest(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));

            HttpWebRequest = WebRequest.Create("https://api.wit.ai/speech") as HttpWebRequest ?? throw new InvalidOperationException("WebRequest is null");
            HttpWebRequest.Method = "POST";
            HttpWebRequest.Headers["Authorization"] = "Bearer " + Token;
            HttpWebRequest.Headers["Transfer-encoding"] = "chunked";
            //HttpWebRequest.PreAuthenticate = true;
            HttpWebRequest.SendChunked = true;
            //HttpWebRequest.AllowWriteStreamBuffering = false;
            //HttpWebRequest.AllowReadStreamBuffering = false;
            HttpWebRequest.ContentType = "audio/wav";
            //HttpWebRequest.ContentLength = 6_000_000;

            Stream = HttpWebRequest.GetRequestStream();

            WriteTask = Task.Run(async () =>
            {
                try
                {
                    using var writer = new BinaryWriter(Stream);

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
                }
                finally
                {
                    Stream.Close();
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
            IsStopped = true;

            await WriteTask.ConfigureAwait(false);

            var response = await HttpWebRequest.GetResponseAsync();
            var stream = response.GetResponseStream() ?? throw new InvalidOperationException("Response stream is null");
            var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

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
                Stream.Dispose();
                WriteTask.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
