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

        private ActionsManager ActionsManager { get; set; } = new ActionsManager
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
            ActionsManager.Import(CommandsStorage.Data);

            Hook.KeyUpEvent += Global_KeyUp;
            Hook.KeyDownEvent += Global_KeyDown;

            ConsoleManager.ActionsManager = ActionsManager;
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

            ActionsManager?.Dispose();
            ActionsManager = null;
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

        private void RecordButton_Click(object sender, RoutedEventArgs e) => ActionsManager.ChangeWithTimeout(3000);

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();

            CommandsStorage.Data = ActionsManager.Export();
            window.ShowDialog();

            ActionsManager.Import(CommandsStorage.Data);
        }

        private void OnNewText(object source, VoiceActionsEventArgs e) => Dispatcher.Invoke(() => {
            var text = e.Text;
            if (string.IsNullOrWhiteSpace(text) || text.Contains("The remote server returned an error: (400) Bad Request"))
            {
                Print("Bad or empty request");
                return;
            }

            Print(ActionsManager.IsHandled(text)
                ? $"Run action for text: \"{text}\""
                : $"We don't have handler for text: \"{text}\"");
        });

        private void Global_KeyUp(KeyboardHookEventArgs e)
        {
            if ((int)e.Key == 192)
            {
                ActionsManager.Stop();
            }
        }

        private void Global_KeyDown(KeyboardHookEventArgs e)
        {
            if ((int)e.Key == 192)
            {
                ActionsManager.Start();
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