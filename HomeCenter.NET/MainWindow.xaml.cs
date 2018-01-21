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

namespace HomeCenter.NET
{
    public partial class MainWindow : IDisposable
    {
        #region Properties

        private ActionsManager ActionsManager { get; set; } = new ActionsManager
        {
            Recorder = new AutoStopRecorder(new WinmmRecorder(), 3000),
            Converter = new YandexConverter("1ce29818-0d15-4080-b6a1-ea5267c9fefd") { Lang = "ru-RU" }
        };

        private Hook Hook { get; } = new Hook("Global Action Hook");

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

            Hook.KeyUpEvent += Global_KeyUp;
            Hook.KeyDownEvent += Global_KeyDown;

            ActionsManager.Import(Settings.Default.CommandsData);
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

        #region Event handlers

        private void InputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && InputTextBox.Text.Length > 0)
            {
                ProcessConsoleCommand(InputTextBox.Text);
                InputTextBox.Clear();
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e) => ActionsManager.Change();

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();

            Settings.Default.CommandsData = ActionsManager.Export();
            window.ShowDialog();

            ActionsManager.Import(Settings.Default.CommandsData);
        }

        private void OnNewText(object source, VoiceActionsEventArgs e) => Dispatcher.Invoke(() => {
            var text = e.Text;
            if (string.IsNullOrWhiteSpace(text) || text.Contains("The remote server returned an error: (400) Bad Request"))
            {
                Console("Bad or empty request");
                return;
            }

            Console(ActionsManager.IsHandled(text)
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
                ActionsManager.Start(true);
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

        #region Private methods

        private void Console(string text) => ConsoleTextBox.Text += $"{DateTime.Now:T}: {text}{Environment.NewLine}";

        public static (string, string) GetPrefixPostfix(string command, char separator)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentNullException(nameof(command));
            }

            var prefix = command.Split(separator).FirstOrDefault();
            var postfix = command.Replace(prefix ?? "", string.Empty).Trim();

            return (prefix, postfix);
        }

        private void ProcessConsoleCommand(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (!text.StartsWith("/"))
            {
                ActionsManager.ProcessText(text);
                return;
            }

            (var prefix, var postfix) = GetPrefixPostfix(text, ' ');
            var command = prefix.ToLowerInvariant().Substring(1);
            switch (command)
            {
                case "add":
                    (var name, var arguments) = GetPrefixPostfix(postfix, ' ');
                    ActionsManager.SetCommand(name, arguments);
                    Console($"Command \"{arguments}\" added");
                    break;

                case "show":
                    Console($@"Current commands:
{string.Join(Environment.NewLine, ActionsManager.GetCommands().Select(pair => $"{pair.Item1} {pair.Item2}"))}");
                    break;

                default:
                    Console($"Command is not exists: {command}");
                    break;

            }

        }

        #endregion

    }
}