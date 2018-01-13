using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Recorders;

namespace VoiceActions.NET.Wpf.Example
{
    public partial class MainWindow : IDisposable
    {
        #region Properties

        private VoiceManager VoiceManager { get; set; } = new VoiceManager
        {
            Recorder = new AutoStopRecorder<WinmmRecorder>(3000),
            Converter = new WitAiConverter("OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS")
        };

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            InputTextBox.Focus();

            VoiceManager.NewText += OnNewText;
            VoiceManager.Started += (sender, args) => Dispatcher.Invoke(() =>
            {
                RecordButton.Content = "🔊";
                RecordButton.Background = Brushes.LightSkyBlue;
            });
            VoiceManager.Stopped += (sender, args) => Dispatcher.Invoke(() =>
            {
                RecordButton.Content = "🔉";
                RecordButton.Background = Brushes.LightGray;
            });
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            VoiceManager?.Dispose();
            VoiceManager = null;
        }

        #endregion

        #region Event handlers

        private void InputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && InputTextBox.Text.Length > 0)
            {
                VoiceManager.ProcessText(InputTextBox.Text);
                InputTextBox.Text = string.Empty;
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e) => VoiceManager.Change();

        private void OnNewText(object source, VoiceActionsEventArgs e) => Dispatcher.Invoke(() => {
            var text = e.Text;
            if (text.Contains("The remote server returned an error: (400) Bad Request"))
            {
                Console("Bad or empty request");
                return;
            }

            Console(!e.IsHandled ? $"We don't have handler for this text: {text}" : $"Run action for text: {text}");
        });

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde)
            {
                e.Handled = true;
                VoiceManager.Stop();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde)
            {
                e.Handled = true;
                VoiceManager.Start(true);
            }
        }

        #endregion

        #region Private methods

        private void Console(string text) => ConsoleTextBox.Text += $"{DateTime.Now:T}: {text}{Environment.NewLine}";

        #endregion

    }
}
