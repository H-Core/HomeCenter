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
            AddEnumerableSetting(nameof(Lang), o => Lang = o, NoEmpty, new []{ "en-US", "ru-RU", "uk-UK", "tr-TR" });
            AddEnumerableSetting(nameof(Format), o => Format = o, NoEmpty, new[] { "wav", "mp3", "opus" });
            AddEnumerableSetting(nameof(Speaker), o => Speaker = o, NoEmpty, new[] { "oksana", "jane", "alyss", "omazh", "zahar", "ermil" });
            AddEnumerableSetting(nameof(Emotion), o => Emotion = o, NoEmpty, new[] { "good", "evil", "neutral" });
            AddEnumerableSetting(nameof(Quality), o => Quality = o, NoEmpty, new[] { "hi", "lo" });
            AddEnumerableSetting(nameof(Speed), o => Speed = o, NoEmpty, new[] { "1.0", "0.1", "0.25", "0.5", "0.75", "1.25", "1.5", "2.0", "3.0" });
        }

        #endregion

        #region Protected methods

        protected override async Task<byte[]> InternalConvert(string text)
        {
            try
            {
                var lang = TextToLang(ref text) ?? Lang;
                using (var client = new HttpClient())
                {
                    return await client.GetByteArrayAsync(
                        $"https://tts.voicetech.yandex.net/generate?text={text}&format={Format}&lang={lang}&speaker={Speaker}&emotion={Emotion}&key={Key}&quality={Quality}&speed={Speed}");
                }
            }
            catch (Exception exception)
            {
                Exception = exception;
                return null;
            }
        }

        protected override string TextToKey(string text) => $"{text}_{Speaker}_{Lang}_{Emotion}_{Speed}_{Format}_{Quality}";

        private static string TextToLang(ref string text)
        {
            if (text.Contains("[EN]"))
            {
                text = text.Replace("[EN]", string.Empty).TrimStart();

                return "en-US";
            }

            return null;
        }

        #endregion
    }
}
