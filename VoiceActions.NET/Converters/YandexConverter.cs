using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public async Task<string> Convert(byte[] bytes)
        {
            using (var client = new HttpClient())
            {
                var content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("audio/x-wav");

                var message = await client.PostAsync($"https://asr.yandex.net/asr_xml?uuid={Uuid}&key={Key}&topic={Topic}&lang={Lang}", content);

                (var text, var exception) = await message.GetResponseText();
                if (string.IsNullOrWhiteSpace(text))
                {
                    Exception = exception;
                    return null;
                }

                var document = XDocument.Parse(text);
                var variants = document.Descendants("variant").Select(element => element.Value);

                return variants.LastOrDefault();
            }
        }
        
        #endregion
    }
}
