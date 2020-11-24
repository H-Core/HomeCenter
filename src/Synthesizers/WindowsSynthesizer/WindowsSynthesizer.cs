using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Synthesizers;
using SpeechLib;

namespace H.NET.Synthesizers
{
    /// <summary>
    /// TODO: DISPOSE PROBLEMS
    /// </summary>
    public class WindowsSynthesizer : Synthesizer, ISynthesizer
    {
        #region Properties

        public string Speaker { get; set; } = string.Empty;
        public string Speed { get; set; } = string.Empty;

        private SpVoice SpVoice { get; } = new ();

        #endregion

        #region Constructors

        public WindowsSynthesizer()
        {
            var voices = SpVoice.GetVoices().OfType<ISpeechObjectToken>().Select(i => i.GetDescription()).ToArray();
            AddEnumerableSetting(nameof(Speaker), o => Speaker = o, NoEmpty, voices);

            AddEnumerableSetting(nameof(Speed), o => Speed = o, NoEmpty, new[] { "1.0", "0.1", "0.25", "0.5", "0.75", "1.25", "1.5", "2.0", "3.0" });
        }

        #endregion

        #region Protected methods

        public Task<byte[]> ConvertAsync(string text, CancellationToken cancellationToken = default)
        {
            try
            {
                SpVoice.Voice = SpVoice.GetVoices().OfType<SpObjectToken>().FirstOrDefault(i => i.GetDescription() == Speaker) ?? SpVoice.Voice;
                SpVoice.Rate = (int)(double.TryParse(Speed, out var result) ? result : 1);

                SpVoice.Speak(text);

                return Task.FromResult(Array.Empty<byte>());
            }
            catch (Exception exception)
            {
                return Task.FromException<byte[]>(exception);
            }
        }

        #endregion
    }
}
