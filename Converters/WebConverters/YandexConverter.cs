using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using H.NET.Converters.Utilities;
using H.NET.Core.Converters;

namespace H.NET.Converters
{
    public class YandexConverter : Converter
    {
        #region Properties

        public string Key { get; set; }
        public string Uuid { get; set; }
        public string Topic { get; set; }
        public string Lang { get; set; }
        public string DisableAntimat { get; set; }

        #endregion

        #region Constructors

        public YandexConverter()
        {
            AddSetting(nameof(Key), o => Key = o, NoEmpty, string.Empty);
            AddSetting(nameof(Uuid), o => Uuid = o, NoEmpty, Guid.NewGuid().ToString().Replace("-", string.Empty));
            AddEnumerableSetting(nameof(Topic), o => Topic = o, NoEmpty, new[] { "queries", "maps", "dates", "names", "numbers", "music", "buying" });
            AddEnumerableSetting(nameof(Lang), o => Lang = o, NoEmpty, new []{ "en-US", "ru-RU", "uk-UK", "tr-TR" });
            AddEnumerableSetting(nameof(DisableAntimat), o => DisableAntimat = o, NoEmpty, new[] { "false", "true" });
        }

        #endregion

        #region Public methods

        public override async Task<string> Convert(byte[] bytes)
        {
            using (var client = new HttpClient())
            {
                var content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("audio/x-wav");

                var message = await client.PostAsync($"https://asr.yandex.net/asr_xml?uuid={Uuid}&key={Key}&topic={Topic}&lang={Lang}&disableAntimat={DisableAntimat}", content);

                var info = await message.GetResponseText();
                if (string.IsNullOrWhiteSpace(info.Text))
                {
                    throw info.Exception;
                }

                var document = XDocument.Parse(info.Text);
                var variants = document.Descendants("variant").Select(element => element.Value);

                return variants.LastOrDefault();
            }
        }
        
        #endregion
    }
}
