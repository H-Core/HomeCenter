using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core;
using H.NET.Core.Converters;
using Newtonsoft.Json;

#nullable enable

namespace H.NET.Converters
{
    public class WitAiConverter : Converter, IConverter
    {
        #region Properties

        bool IConverter.IsStreamingRecognitionSupported => true;

        public string Token { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public WitAiConverter()
        {
            AddSetting(nameof(Token), o => Token = o, NoEmpty, string.Empty);
        }

        #endregion

        #region Public methods

        public override Task<IStreamingRecognition> StartStreamingRecognitionAsync(CancellationToken _ = default)
        {
            return Task.FromResult<IStreamingRecognition>(new WitAiStreamingRecognition(Token));
        }

        public override async Task<string> ConvertAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.wit.ai/speech?v=20200103")
            {
                Headers =
                {
                    {"Authorization", $"Bearer {Token}"},
                    {"Transfer-encoding", "chunked"},
                },
                Content = new ByteArrayContent(bytes)
                {
                    Headers =
                    {
                        {"Content-Type", "audio/wav"},
                    }
                },
            };
            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Invalid response: {json}");
            }

            var obj = JsonConvert.DeserializeObject<WitAiResponse>(json);

            return obj.Text ?? string.Empty;
            //return await ConvertOverStreamingRecognition(bytes, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
