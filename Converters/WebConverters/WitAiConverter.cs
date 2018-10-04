using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using H.NET.Converters.Utilities;
using H.NET.Core.Converters;
using Newtonsoft.Json;

namespace H.NET.Converters
{
    public class WitAiConverter : Converter
    {
        #region Properties

        public string Token { get; set; }

        #endregion

        #region Constructors

        public WitAiConverter()
        {
            AddSetting(nameof(Token), o => Token = o, NoEmpty, string.Empty);
        }

        #endregion

        #region Public methods

        public override async Task<string> Convert(byte[] bytes)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + Token);

                var content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");

                var message = await client.PostAsync("https://api.wit.ai/speech", content);

                var info = await message.GetResponseText();
                if (string.IsNullOrWhiteSpace(info.Text) || string.Equals(info.Text, "The remote server returned an error: (400) Bad Request"))
                {
                    Exception = info.Exception;
                    return null;
                }

                var obj = JsonConvert.DeserializeObject<RootObject>(info.Text);

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
