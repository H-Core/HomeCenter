using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoiceActions.NET.Converters.Core;

namespace VoiceActions.NET.Converters
{
    public class YandexConverter : BaseConverter, IConverter
    {
        #region Properties

        public string Key { get; }
        public string Uuid { get; } = Guid.NewGuid().ToString().Replace("-", string.Empty);
        public string Topic { get; set; } = "queries";
        public string Lang { get; set; } = "en-US";
        
        #endregion

        #region Constructors

        public YandexConverter(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        #endregion

        #region Public methods

        public async Task<string> Convert(byte[] bytes) => await ProcessSpokenText(bytes);

        #endregion

        #region Private methods

        private Task<string> ProcessSpokenText(byte[] bytes) => Task.Run(() => ProcessSpeech(bytes));

        private string ProcessSpeech(byte[] bytes)
        {
            var request = (HttpWebRequest)WebRequest.Create($"https://asr.yandex.net/asr_xml?uuid={Uuid}&key={Key}&topic={Topic}&lang={Lang}");
            request.Method = "POST";
            request.ContentType = "audio/x-wav";
            request.ContentLength = bytes.Length;
            request.GetRequestStream().Write(bytes, 0, bytes.Length);

            (var text, var exception) = request.GetResponseText();
            if (string.IsNullOrWhiteSpace(text))
            {
                return exception.Message;
            }

            var document = XDocument.Parse(text);
            var variants = document.Descendants("variant").Select(element => element.Value);

            return variants.LastOrDefault();
        }
        
        #endregion
    }
}
