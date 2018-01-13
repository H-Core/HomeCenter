using System;
using System.Threading.Tasks;
using VoiceActions.NET.SpeechToTextConverters.Core.WitAiConverter;

namespace VoiceActions.NET.SpeechToTextConverters
{
    public class WitAiConverter : ISpeechToTextConverter
    {
        #region Properties

        private string Token { get; }

        #endregion

        #region Constructors

        public WitAiConverter(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        #endregion

        #region Public methods

        public async Task<string> Convert(byte[] bytes)
        {
            var client = new WitAiClient(Token);

            return await client.ProcessSpokenText(bytes);
        }

        #endregion
    }
}
