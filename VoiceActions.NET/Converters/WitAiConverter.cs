using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VoiceActions.NET.Converters.Core;

namespace VoiceActions.NET.Converters
{
    public class WitAiConverter : BaseConverter, IConverter
    {
        #region Properties

        private string Token { get; }

        #endregion

        #region Constructors

        public WitAiConverter(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        #endregion

        #region Public methods

        public async Task<string> Convert(byte[] bytes)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + Token);

                var content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");

                var message = await client.PostAsync("https://api.wit.ai/speech", content);

                (var text, var exception) = await message.GetResponseText();
                if (string.IsNullOrWhiteSpace(text) || string.Equals(text, "The remote server returned an error: (400) Bad Request"))
                {
                    Exception = exception;
                    return null;
                }

                var obj = JsonConvert.DeserializeObject<RootObject>(text);

                return obj._text;
            }
        }

        private class RootObject
        {
            public string _text { get; set; }
            public object entities { get; set; }
            public string msg_id { get; set; }
        }

        #endregion
    }
}
