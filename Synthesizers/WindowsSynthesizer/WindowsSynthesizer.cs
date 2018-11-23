using System;
using System.Linq;
using System.Threading.Tasks;
using H.NET.Core.Synthesizers;
using SpeechLib;

namespace H.NET.Synthesizers
{
    public class WindowsSynthesizer : Synthesizer
    {
        #region Properties

        public string Speaker { get; set; } 
        public string Speed { get; set; }

        private SpVoice Voice { get; } = new SpVoice();

        #endregion

        #region Constructors

        public WindowsSynthesizer()
        {
            var voices = Voice.GetVoices().OfType<ISpeechObjectToken>().Select(i => i.GetDescription()).ToArray();
            AddEnumerableSetting(nameof(Speaker), o => Speaker = o, NoEmpty, voices);

            AddEnumerableSetting(nameof(Speed), o => Speed = o, NoEmpty, new[] { "1.0", "0.1", "0.25", "0.5", "0.75", "1.25", "1.5", "2.0", "3.0" });
        }

        #endregion

        #region Protected methods

        protected override async Task<byte[]> InternalConvert(string text)
        {
            try
            {
                Voice.Voice = Voice.GetVoices().OfType<SpObjectToken>().FirstOrDefault(i => i.GetDescription() == Speaker) ?? Voice.Voice;
                Voice.Rate = (int)(double.TryParse(Speed, out var result) ? result : 1);

                Voice.Speak(text);

                return await Task.FromResult(new byte[0]);
            }
            catch (Exception exception)
            {
                Exception = exception;
                return null;
            }
        }

        private static string TextToLang(ref string text)
        {
            if (text.TrimStart().StartsWith("[EN]"))
            {
                text = text.TrimStart().Replace("[EN]", string.Empty);

                return "en-US";
            }

            return null;
        }

        #endregion
    }
}
