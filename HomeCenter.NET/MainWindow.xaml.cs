using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HomeCenter.NET.Utilities;
using VoiceActions.NET;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Recorders;

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
            Manager.Import(CommandsStorage.Data);

            Hook.KeyUpEvent += Global_KeyUp;
            Hook.KeyDownEvent += Global_KeyDown;

            ConsoleManager.Manager = Manager;
            ConsoleManager.NewOutput += (o, args) => Print(args.Text);
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

        #endregion

        #region Event handlers

        private void InputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && InputTextBox.Text.Length > 0)
            {
                ConsoleManager.Run(InputTextBox.Text);
                InputTextBox.Clear();
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

        private void ShowHide_Click(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

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

        #endregion

    }
}