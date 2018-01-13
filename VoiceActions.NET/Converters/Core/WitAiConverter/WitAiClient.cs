using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VoiceActions.NET.Converters.Core.WitAiConverter
{
    public class WitAiClient
    {
        public class Entities
        {
        }

        public class RootObject
        {
            public string _text { get; set; }
            public Entities entities { get; set; }
            public string msg_id { get; set; }
        }

        #region Properties

        private string Token { get; }

        #endregion

        #region Constructors

        public WitAiClient(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        #endregion

        #region Public methods

        public Task<string> ProcessSpokenText(byte[] bytes) => Task.Run(() => ProcessSpeech(bytes));

        #endregion

        #region Private methods

        private static string GetResponseText(WebRequest request)
        {
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return $"Error: {response.StatusCode}";
                }

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        return "Error: GetResponseStream return null";
                    }

                    using (var responseReader = new StreamReader(responseStream))
                    {
                        var json = responseReader.ReadToEnd();

                        var obj = JsonConvert.DeserializeObject<RootObject>(json);
                        return obj._text;
                    }
                }
            }
            catch (Exception exception)
            {
                return $"Error: {exception.Message}";
            }
        }

        private string ProcessSpeech(byte[] bytes)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.wit.ai/speech");
            request.Method = "POST";
            request.Headers["Authorization"] = "Bearer " + Token;
            request.ContentType = "audio/wav";
            request.ContentLength = bytes.Length;
            request.GetRequestStream().Write(bytes, 0, bytes.Length);

            return GetResponseText(request);
        }

        #endregion
    }
}
