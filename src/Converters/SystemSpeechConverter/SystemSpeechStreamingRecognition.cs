using System;
using System.Threading;
using System.Threading.Tasks;
using H.Core.Converters;
using Microsoft.Speech.Recognition;

namespace H.NET.Converters
{
    public sealed class SystemSpeechStreamingRecognition : StreamingRecognition
    {
        #region Properties

        private SpeechRecognitionEngine SpeechRecognitionEngine { get; }

        #endregion

        #region Constructors

        internal SystemSpeechStreamingRecognition(SpeechRecognitionEngine speechRecognitionEngine)
        {
            SpeechRecognitionEngine = speechRecognitionEngine ?? throw new ArgumentNullException(nameof(speechRecognitionEngine));

            SpeechRecognitionEngine.SpeechHypothesized += (_, args) => OnAfterPartialResults(args.Result.Text);
            SpeechRecognitionEngine.SpeechRecognized += (_, args) => OnAfterFinalResults(args.Result.Text);

            SpeechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        #endregion

        #region Public methods

        public override Task WriteAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken = default)
        {
            SpeechRecognitionEngine.RecognizeAsyncStop();

            return Task.CompletedTask;
        }

        #endregion
    }
}
