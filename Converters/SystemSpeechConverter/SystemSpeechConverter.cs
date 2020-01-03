using System.Diagnostics;
using System.Globalization;
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

        bool IConverter.IsStreamingRecognitionSupported => false;

        private SpeechRecognitionEngine SpeechRecognitionEngine { get; }

        #endregion

        #region Constructors

        public SystemSpeechConverter()
        {
            foreach (var info in SpeechRecognitionEngine.InstalledRecognizers())
            {
                Trace.WriteLine($"Recognizer: Name: {info.Name} Language: {info.Culture.Name}");
            }
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
            SpeechRecognitionEngine.SpeechHypothesized += (sender, args) => Trace.WriteLine($"SpeechHypothesized: {args.Result.Text}");
            SpeechRecognitionEngine.SpeechRecognized += (sender, args) => Trace.WriteLine($"SpeechRecognized: {args.Result.Text}");
        }

        #endregion

        #region Public methods

        public override Task<string> ConvertAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            SpeechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);

            return Task.FromResult(string.Empty);
        }

        public override void Dispose()
        {
            SpeechRecognitionEngine.Dispose();
        }

        #endregion
    }
}
