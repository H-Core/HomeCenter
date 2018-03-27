using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace H.NET.Synthesizers
{
    public class YandexSynthesizer : CacheableSynthesizer
    {
        #region Properties

        public string Key { get; set; }
        public string Lang { get; set; }
        public string Format { get; set; }
        public string Speaker { get; set; } 
        public string Emotion { get; set; }
        public string Quality { get; set; }
        public string Speed { get; set; }

        #endregion

        #region Constructors

        public YandexSynthesizer()
        {
            AddSetting(nameof(Key), o => Key = o, NoEmpty, string.Empty);
            AddSetting(nameof(Lang), o => Lang = o, NoEmpty, "en-US");
            AddSetting(nameof(Format), o => Format = o, NoEmpty, "wav");
            AddSetting(nameof(Speaker), o => Speaker = o, NoEmpty, "oksana");
            AddSetting(nameof(Emotion), o => Emotion = o, NoEmpty, "good");
            AddSetting(nameof(Quality), o => Quality = o, NoEmpty, "hi");
            AddSetting(nameof(Speed), o => Speed = o, NoEmpty, "1.0");
        }

        #endregion

        #region Protected methods

        protected override async Task<byte[]> InternalConvert(string text)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    return await client.GetByteArrayAsync(
                        $"https://tts.voicetech.yandex.net/generate?text={text}&format={Format}&lang={Lang}&speaker={Speaker}&emotion={Emotion}&key={Key}&quality={Quality}&speed={Speed}");
                }
            }
            catch (Exception exception)
            {
                Exception = exception;
                return null;
            }
        }

        protected override string TextToKey(string text) => $"{text}_{Speaker}_{Lang}_{Emotion}_{Speed}_{Format}_{Quality}";

        #endregion
    }
}
