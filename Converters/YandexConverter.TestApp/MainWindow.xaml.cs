using System;
using System.Threading.Tasks;
using System.Windows;
using H.NET.Recorders;

namespace H.NET.Converters.TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcessButton.IsEnabled = false;
            OutputTextBox.Text += $"{DateTime.Now:h:mm:ss.fff} Started {Environment.NewLine}";

            try
            {
                using var recorder = new NAudioRecorder();
                recorder.Start();

                using var converter = new YandexConverter
                {
                    OAuthToken = OAuthTokenTextBox.Text,
                    FolderId = FolderIdTextBox.Text,
                    Lang = "ru-RU",
                    SampleRateHertz = 8000,
                };

                using var recognition = await converter.StartStreamingRecognitionAsync().ConfigureAwait(false);
                recognition.AfterPartialResults += (o, args) => Dispatcher?.Invoke(() =>
                {
                    OutputTextBox.Text += $"{DateTime.Now:h:mm:ss.fff} {args.Text}{Environment.NewLine}";
                    OutputTextBlock.Text = $"{DateTime.Now:h:mm:ss.fff} {args.Text}";
                });
                recognition.AfterFinalResults += (o, args) => Dispatcher?.Invoke(() =>
                {
                    OutputTextBox.Text += $"{DateTime.Now:h:mm:ss.fff} {args.Text}{Environment.NewLine}";
                    OutputTextBlock.Text = $"{DateTime.Now:h:mm:ss.fff} {args.Text}";
                });

                await recognition.WriteAsync(recorder.Data);

                // ReSharper disable once AccessToDisposedClosure
                recorder.NewData += async (o, args) => await recognition.WriteAsync(args.Buffer).ConfigureAwait(false);

                await Task.Delay(TimeSpan.FromMilliseconds(5000)).ConfigureAwait(false);

                recorder.Stop();
                await recognition.StopAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }

            Dispatcher?.Invoke(() =>
            {
                ProcessButton.IsEnabled = true;
                OutputTextBox.Text += $"{DateTime.Now:h:mm:ss.fff} Ended {Environment.NewLine}";
            });
        }
    }
}
