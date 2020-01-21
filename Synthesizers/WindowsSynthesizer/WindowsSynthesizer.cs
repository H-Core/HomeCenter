using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core;
using H.NET.Core.Synthesizers;
using SpeechLib;

namespace H.NET.Synthesizers
{
    /// <summary>
    /// TODO: DISPOSE PROBLEMS
    /// </summary>
    public class WindowsSynthesizer : Synthesizer, ISynthesizer
    {
        #region Properties

        public string Speaker { get; set; } 
        public string Speed { get; set; }

        private SpVoice SpVoice { get; } = new SpVoice();

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

                return Task.FromResult<byte[]>(null);
            }
            catch (Exception exception)
            {
                return Task.FromException<byte[]>(exception);
            }
        }

        #endregion
    }
}
