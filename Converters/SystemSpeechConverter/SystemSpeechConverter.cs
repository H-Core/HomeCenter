using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core;
using H.NET.Core.Converters;
using Microsoft.Speech.Recognition;

namespace H.NET.Converters
{
    public sealed class SystemSpeechConverter : Converter, IConverter
    {
        #region Properties

        bool IConverter.IsStreamingRecognitionSupported => true;

        private SpeechRecognitionEngine SpeechRecognitionEngine { get; }

        public string Recognizer { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public SystemSpeechConverter()
        {
            AddEnumerableSetting(nameof(Recognizer), o => Recognizer = o, NoEmpty, SpeechRecognitionEngine.InstalledRecognizers().Select(i => i.Name).ToArray());

            SpeechRecognitionEngine = new SpeechRecognitionEngine(new CultureInfo("ru-RU"));
            SpeechRecognitionEngine.SetInputToDefaultAudioDevice();

            var builder = new GrammarBuilder
            {
                Culture = new CultureInfo("ru-RU"),
            };

            builder.Append("Бот");
            builder.Append(new Choices("сделай", "принеси"));
            builder.Append(new Choices("справочник", "отчет"));

            SpeechRecognitionEngine.LoadGrammar(new Grammar(builder));
        }

        #endregion

        #region Public methods

        public override Task<IStreamingRecognition> StartStreamingRecognitionAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IStreamingRecognition>(new SystemSpeechStreamingRecognition(SpeechRecognitionEngine));
        }

        public override Task<string> ConvertAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            SpeechRecognitionEngine.Dispose();
        }

        #endregion
    }
}
