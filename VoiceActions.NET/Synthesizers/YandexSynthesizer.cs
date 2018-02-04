using System;
using System.Net.Http;
using System.Threading.Tasks;
using VoiceActions.NET.Synthesizers.Core;

namespace VoiceActions.NET.Synthesizers
{
    public class YandexSynthesizer : BaseSynthesizer, ISynthesizer
    {
        #region Properties

        public string Key { get; }
        public string Lang { get; set; } = "en-US";
        public string Format { get; set; } = "wav";
        public string Speaker { get; set; } = "oksana";
        public string Emotion { get; set; } = "good";
        public string Quality { get; set; } = "hi";
        public string Speed { get; set; } = "1.0";

        #endregion

        #region Constructors

        public YandexSynthesizer(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        #endregion

        #region Public methods

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
