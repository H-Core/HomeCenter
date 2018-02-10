using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Runners;
using HomeCenter.NET.Storages;
using HomeCenter.NET.Utilities;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Managers;
using VoiceActions.NET.Recorders;
using VoiceActions.NET.Synthesizers;

namespace HomeCenter.NET.Windows
{
    public partial class MainWindow : IDisposable
    {
        #region Properties

        private Manager<Command> Manager { get; set; } = new Manager<Command>(new CommandsStorage())
        {
            Recorder = new WinmmRecorder(),
            Converter = new YandexConverter("1ce29818-0d15-4080-b6a1-ea5267c9fefd") { Lang = "ru-RU" }
        };

        private Hook Hook { get; } = new Hook("Global Action Hook");
        private ConsoleRunner ConsoleRunner { get; set; } = new ConsoleRunner();
        private DefaultRunner Runner { get; set; } = new DefaultRunner();
        private ISynthesizer Synthesizer { get; set; } = new YandexSynthesizer("1ce29818-0d15-4080-b6a1-ea5267c9fefd") { Lang = "ru-RU" };

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
            Runner.NewSpeech += (o, args) => Say(args.Text);
            Manager.NewValue += command => Runner?.Run(command.Data);

            Hook.KeyUpEvent += Global_KeyUp;
            Hook.KeyDownEvent += Global_KeyDown;

            ConsoleRunner.Manager = Manager;
            ConsoleRunner.NewOutput += (o, args) => Print(args.Text);
            ConsoleRunner.NewSpeech += (o, args) => Say(args.Text);
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

            Runner?.Dispose();
            Runner = null;

            ConsoleRunner?.Dispose();
            ConsoleRunner = null;

            Synthesizer?.Dispose();
            Synthesizer = null;
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

                    ConsoleRunner.Run(InputTextBox.Text);
                    InputTextBox.Clear();
                    break;

                case Key.Up:
                    if (ConsoleRunner.History.Any())
                    {
                        InputTextBox.Text = ConsoleRunner.History.LastOrDefault() ?? "";
                        ConsoleRunner.History.RemoveAt(ConsoleRunner.History.Count - 1);
                    }
                    break;
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e) => Manager.ChangeWithTimeout(3000);

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();

            window.ShowDialog();
        }

        private void OnNewText(string text) => Dispatcher.Invoke(() => {
            if (string.IsNullOrWhiteSpace(text) || text.Contains("The remote server returned an error: (400) Bad Request"))
            {
                Print("Bad or empty request");
                return;
            }

            Print(Manager.Storage.ContainsKey(text)
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