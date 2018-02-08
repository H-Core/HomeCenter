using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Utilities;
using VoiceActions.NET;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Recorders;
using VoiceActions.NET.Synthesizers;

namespace HomeCenter.NET
{
    public partial class MainWindow : IDisposable
    {
        #region Properties

        private RunnerManager Manager { get; set; } = new RunnerManager
        {
            Recorder = new WinmmRecorder(),
            Converter = new YandexConverter("1ce29818-0d15-4080-b6a1-ea5267c9fefd") { Lang = "ru-RU" }
        };

        private Hook Hook { get; } = new Hook("Global Action Hook");
        private ConsoleManager ConsoleManager { get; } = new ConsoleManager();
        private ISynthesizer Synthesizer { get; } = new YandexSynthesizer("1ce29818-0d15-4080-b6a1-ea5267c9fefd") { Lang = "ru-RU" };

        private bool CanClose { get; set; }

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            if (Settings.Default.IsStartMinimized)
            {
                Visibility = Visibility.Hidden;
            }

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
            Manager.Import(CommandsStorage.Data);
            Manager.Runner.NewSpeech += (o, args) => Say(args.Text);

            Hook.KeyUpEvent += Global_KeyUp;
            Hook.KeyDownEvent += Global_KeyDown;

            ConsoleManager.Manager = Manager;
            ConsoleManager.NewOutput += (o, args) => Print(args.Text);
            ConsoleManager.NewSpeech += (o, args) => Say(args.Text);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (TaskbarIcon != null)
            {
                TaskbarIcon.Icon = null;
                TaskbarIcon.Visibility = Visibility.Hidden;
                TaskbarIcon.Dispose();
                TaskbarIcon = null;
            }

            Manager?.Dispose();
            Manager = null;
        }

        #endregion

        #region Private methods

        private void Print(string text) => ConsoleTextBox.Text += $"{DateTime.Now:T}: {text}{Environment.NewLine}";

        private static void Say(byte[] bytes) => bytes?.Play();
        private async void Say(string text) => Say(await Synthesizer.Convert(text));

        #endregion

        #region Event handlers

        private void InputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (InputTextBox.Text.Length == 0)
                    {
                        break;
                    }

                    ConsoleManager.Run(InputTextBox.Text);
                    InputTextBox.Clear();
                    break;

                case Key.Up:
                    if (ConsoleManager.History.Any())
                    {
                        InputTextBox.Text = ConsoleManager.History.LastOrDefault() ?? "";
                        ConsoleManager.History.RemoveAt(ConsoleManager.History.Count - 1);
                    }
                    break;
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e) => Manager.ChangeWithTimeout(3000);

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();

            CommandsStorage.Data = Manager.Export();
            window.ShowDialog();

            Manager.Import(CommandsStorage.Data);
        }

        private void OnNewText(object source, VoiceActionsEventArgs e) => Dispatcher.Invoke(() => {
            var text = e.Text;
            if (string.IsNullOrWhiteSpace(text) || text.Contains("The remote server returned an error: (400) Bad Request"))
            {
                Print("Bad or empty request");
                return;
            }

            Print(Manager.IsHandled(text)
                ? $"Run action for text: \"{text}\""
                : $"We don't have handler for text: \"{text}\"");
        });

        private void Global_KeyUp(KeyboardHookEventArgs e)
        {
            if ((int)e.Key == 192)
            {
                Manager.Stop();
            }
        }

        private void Global_KeyDown(KeyboardHookEventArgs e)
        {
            if ((int)e.Key == 192)
            {
                Manager.Start();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CanClose = true;
            Close();
        } 

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde)
            {
                e.Handled = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde)
            {
                e.Handled = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!CanClose)
            {
                Visibility = Visibility.Hidden;
                e.Cancel = true;
            }
        }

        #endregion
    }
}