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
        
        #endregion

        #region Constructors

        public YandexConverter()
        {
            AddSetting(nameof(Key), o => Key = o, NoEmpty, string.Empty);
            AddSetting(nameof(Uuid), o => Uuid = o, NoEmpty, Guid.NewGuid().ToString().Replace("-", string.Empty));
            AddSetting(nameof(Topic), o => Topic = o, NoEmpty, "queries");
            AddSetting(nameof(Lang), o => Lang = o, NoEmpty, "en-US");
        }

        #endregion

        #region Public methods

        public override async Task<string> Convert(byte[] bytes)
        {
            using (var client = new HttpClient())
            {
                var content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("audio/x-wav");

                var message = await client.PostAsync($"https://asr.yandex.net/asr_xml?uuid={Uuid}&key={Key}&topic={Topic}&lang={Lang}", content);

                var info = await message.GetResponseText();
                if (string.IsNullOrWhiteSpace(info.Text))
                {
                    Exception = info.Exception;
                    return null;
                }

                var document = XDocument.Parse(info.Text);
                var variants = document.Descendants("variant").Select(element => element.Value);

                return variants.LastOrDefault();
            }
        }
        
        #endregion
    }
}
