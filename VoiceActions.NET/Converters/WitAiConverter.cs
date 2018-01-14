using System;
using System.Net;
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

        public async Task<string> Convert(byte[] bytes) => await ProcessSpokenText(bytes);

        #endregion

        #region Private methods

        public class Entities
        {
        }

        public class RootObject
        {
            public string _text { get; set; }
            public Entities entities { get; set; }
            public string msg_id { get; set; }
        }

        private Task<string> ProcessSpokenText(byte[] bytes) => Task.Run(() => ProcessSpeech(bytes));
        
        private string ProcessSpeech(byte[] bytes)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.wit.ai/speech");
            request.Method = "POST";
            request.Headers["Authorization"] = "Bearer " + Token;
            request.ContentType = "audio/wav";
            request.ContentLength = bytes.Length;
            request.GetRequestStream().Write(bytes, 0, bytes.Length);

            (var text, var exception) = request.GetResponseText();
            if (string.IsNullOrWhiteSpace(text))
            {
                return exception.Message;
            }

            var obj = JsonConvert.DeserializeObject<RootObject>(text);

            return obj._text;
        }

        #endregion
    }
}
