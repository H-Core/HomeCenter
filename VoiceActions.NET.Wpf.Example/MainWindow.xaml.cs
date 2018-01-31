using System;
using System.Diagnostics;
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

        private ActionsManager Manager { get; set; } = new ActionsManager
        {
            Recorder = new WinmmRecorder(),
            Converter = new YandexConverter("1ce29818-0d15-4080-b6a1-ea5267c9fefd"){ Lang = "ru-RU" }
        };

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            InputTextBox.Focus();

            Manager.NewText += OnNewText;
            Manager.Started += (sender, args) => Dispatcher.Invoke(() =>
            {
                RecordButton.Content = "🔊";
                RecordButton.Background = Brushes.LightSkyBlue;
            });
            Manager.Stopped += (sender, args) => Dispatcher.Invoke(() =>
            {
                RecordButton.Content = "🔉";
                RecordButton.Background = Brushes.LightGray;
            });

            Manager.Actions["открой диспетчер файлов"] = () => Process.Start("explorer.exe", "C:/");
            Manager.Actions["проверка"] = () => MessageBox.Show("test");
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Manager?.Dispose();
            Manager = null;
        }

        #endregion

        #region Event handlers

        private void InputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && InputTextBox.Text.Length > 0)
            {
                Manager.ProcessText(InputTextBox.Text);
                InputTextBox.Clear();
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e) => Manager.ChangeWithTimeout(3000);

        private void OnNewText(object source, VoiceActionsEventArgs e) => Dispatcher.Invoke(() => {
            var text = e.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                var message = Manager?.Converter?.Exception?.Message;
                Console($"Bad or empty request. {(!string.IsNullOrWhiteSpace(message) ? $"Message: {message}" : "")}");
                return;
            }

            Console(Manager.IsHandled(text)
                ? $"Run action for text: \"{text}\""
                : $"We don't have handler for text: \"{text}\"");
        });

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde)
            {
                e.Handled = true;
                Manager.Stop();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde)
            {
                e.Handled = true;
                Manager.Start();
            }
        }

        #endregion

        #region Private methods

        private void Console(string text) => ConsoleTextBox.Text += $"{DateTime.Now:T}: {text}{Environment.NewLine}";

        #endregion

    }
}
