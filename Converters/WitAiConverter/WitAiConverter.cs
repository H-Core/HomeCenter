using System.Threading;
using System.Threading.Tasks;
using H.NET.Core;
using H.NET.Core.Converters;

#nullable enable

namespace H.NET.Converters
{
    public class WitAiConverter : Converter
    {
        #region Properties

        public string Token { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public WitAiConverter()
        {
            AddSetting(nameof(Token), o => Token = o, NoEmpty, string.Empty);
        }

        #endregion

        #region Public methods

        public override Task<IStreamingRecognition> StartStreamingRecognitionAsync(CancellationToken _ = default)
        {
            return Task.FromResult<IStreamingRecognition>(new WitAiStreamingRecognition(Token));
        }

        public override async Task<string> ConvertAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            return await ConvertOverStreamingRecognition(bytes, cancellationToken);
        }

        #endregion
    }
}
