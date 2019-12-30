using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using H.NET.Converters.Utilities;
using H.NET.Core;
using H.NET.Core.Converters;
using Newtonsoft.Json;
using Yandex.Cloud.Ai.Stt.V2;

namespace H.NET.Converters
{
    public sealed class YandexConverter : Converter
    {
        #region Properties

        public string Lang { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public bool ProfanityFilter { get; set; }
        public string Format { get; set; } = string.Empty;
        public int SampleRateHertz { get; set; }
        public string FolderId { get; set; } = string.Empty;
        public string OAuthToken { get; set; } = string.Empty;

        public string? IamToken { get; set; }

        #endregion

        #region Constructors

        public YandexConverter()
        {
            AddSetting(nameof(FolderId), o => FolderId = o, Any, string.Empty);
            AddSetting(nameof(OAuthToken), o => OAuthToken = o, NoEmpty, string.Empty);

            AddEnumerableSetting(nameof(Lang), o => Lang = o, NoEmpty, new[] { "ru-RU", "en-US", "uk-UK", "tr-TR" });
            AddEnumerableSetting(nameof(Topic), o => Topic = o, NoEmpty, new[] { "general", "maps", "dates", "names", "numbers" });
            AddEnumerableSetting(nameof(ProfanityFilter), o => ProfanityFilter = o == "true", NoEmpty, new[] { "false", "true" });
            AddEnumerableSetting(nameof(Format), o => Format = o, NoEmpty, new[] { "lpcm", "oggopus" });
            AddEnumerableSetting(nameof(SampleRateHertz), o => SampleRateHertz = int.TryParse(o, out var value) ? value : default, NoEmpty, new[] { "8000", "48000", "16000" });
        }

        #endregion

        #region Public methods

        public override async Task<IStreamingRecognition> StartStreamingRecognitionAsync(CancellationToken cancellationToken = default)
        {
            IamToken ??= await RequestIamTokenByOAuthTokenAsync(OAuthToken, cancellationToken).ConfigureAwait(false);

            var channel = new Channel("stt.api.cloud.yandex.net", 443, new SslCredentials());
            var client = new SttService.SttServiceClient(channel);
            var call = client.StreamingRecognize(new Metadata
            {
                {"authorization", $"Bearer {IamToken}"}
            });

            await call.RequestStream.WriteAsync(new StreamingRecognitionRequest
            {
                Config = new RecognitionConfig
                {
                    Specification = new RecognitionSpec
                    {
                        LanguageCode = Lang,
                        ProfanityFilter = ProfanityFilter,
                        Model = Topic,
                        AudioEncoding = Format switch
                        {
                            "oggopus" => RecognitionSpec.Types.AudioEncoding.OggOpus,
                            "lpcm" => RecognitionSpec.Types.AudioEncoding.Linear16Pcm,
                            _ => RecognitionSpec.Types.AudioEncoding.Unspecified,
                        },
                        SampleRateHertz = SampleRateHertz,
                        PartialResults = true,
                    },
                    FolderId = FolderId,
                }
            }).ConfigureAwait(false);

            return new YandexStreamingRecognition(call);
        }

        public override async Task<string> ConvertAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            IamToken ??= await RequestIamTokenByOAuthTokenAsync(OAuthToken, cancellationToken).ConfigureAwait(false);

            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://stt.api.cloud.yandex.net/speech/v1/stt:recognize").WithQuery(new Dictionary<string, string?>
            {
                { "lang", Lang },
                { "topic", Topic },
                { "profanityFilter", ProfanityFilter ? "true" :  "false" },
                { "format", Format },
                { "sampleRateHertz", $"{SampleRateHertz}" },
                { "folderId", FolderId },
            }))
            {
                Headers =
                {
                    { "Authorization", $"Bearer {IamToken}" },
                    //{ "Transfer-Encoding", "chunked" },
                },
                Content = new ByteArrayContent(bytes)
            };
            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
           
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return dictionary.TryGetValue("result", out var value)
                ? value
                : throw new InvalidOperationException($"Result is not found: {json}");
        }

        #endregion

        #region Private methods

        private static async Task<string> RequestIamTokenByOAuthTokenAsync(string oAuthToken, CancellationToken cancellationToken = default)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://iam.api.cloud.yandex.net/iam/v1/tokens")
            {
                Content = new StringContent(JsonConvert.SerializeObject(new 
                {
                    yandexPassportOauthToken = oAuthToken,
                }), Encoding.UTF8, "application/json"),
            };
            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return dictionary.TryGetValue("iamToken", out var value)
                ? value
                : throw new InvalidOperationException($"Token is not found: {json}");
        }

        #endregion
    }
}
