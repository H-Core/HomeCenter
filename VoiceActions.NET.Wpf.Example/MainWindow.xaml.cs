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

        private ActionsManager ActionsManager { get; set; } = new ActionsManager
        {
            Recorder = new AutoStopRecorder(new WinmmRecorder(), 3000),
            Converter = new YandexConverter("1ce29818-0d15-4080-b6a1-ea5267c9fefd"){ Lang = "ru-RU" }
        };

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            InputTextBox.Focus();

            ActionsManager.NewText += OnNewText;
            ActionsManager.Started += (sender, args) => Dispatcher.Invoke(() =>
            {
                RecordButton.Content = "🔊";
                RecordButton.Background = Brushes.LightSkyBlue;
            });
            ActionsManager.Stopped += (sender, args) => Dispatcher.Invoke(() =>
            {
                RecordButton.Content = "🔉";
                RecordButton.Background = Brushes.LightGray;
            });
            ActionsManager.SetCommand("проверка", "run explorer.exe C:/");
            ActionsManager.SetAction("проверка", () => MessageBox.Show("test"));
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            ActionsManager?.Dispose();
            ActionsManager = null;
        }

        #endregion

        #region Event handlers

        private void InputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && InputTextBox.Text.Length > 0)
            {
                ActionsManager.ProcessText(InputTextBox.Text);
                InputTextBox.Clear();
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e) => ActionsManager.Change();

        private void OnNewText(object source, VoiceActionsEventArgs e) => Dispatcher.Invoke(() => {
            var text = e.Text;
            if (text.Contains("The remote server returned an error: (400) Bad Request"))
            {
                Console("Bad or empty request");
                return;
            }

            Console(ActionsManager.IsHandled(text)
                ? $"Run action for text: \"{text}\""
                : $"We don't have handler for text: \"{text}\"");
        });

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde)
            {
                e.Handled = true;
                ActionsManager.Stop();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde)
            {
                e.Handled = true;
                ActionsManager.Start(true);
            }
        }

        #endregion

        #region Private methods

        private void Console(string text) => ConsoleTextBox.Text += $"{DateTime.Now:T}: {text}{Environment.NewLine}";

        #endregion

    }
}
