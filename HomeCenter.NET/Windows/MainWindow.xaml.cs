using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        private BaseManager Manager { get; set; } = new BaseManager
        {
            Recorder = new WinmmRecorder(),
            Converter = new YandexConverter("1ce29818-0d15-4080-b6a1-ea5267c9fefd") { Lang = "ru-RU" }
        };

        private IConverter AlternativeConverter { get; set; } = new WitAiConverter("OQTI5VZ6JYDHYXTDKCDIYUODEUKH3ELS");

        private Hook Hook { get; } = new Hook("Global Action Hook");
        private GlobalRunner GlobalRunner { get; set; } = new GlobalRunner(new CommandsStorage());
        private ISynthesizer Synthesizer { get; set; } = new YandexSynthesizer("1ce29818-0d15-4080-b6a1-ea5267c9fefd") { Lang = "ru-RU" };

        private bool CanClose { get; set; }

        #region FileSystemWatcher

        public static string SharedDirectory => Directory.CreateDirectory(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "HomeCenter.NET",
                "commands")
        ).FullName;

        private FileSystemWatcher Watcher { get; } = new FileSystemWatcher(SharedDirectory, "*.txt");

        #endregion

        #endregion

        #region Constructors

        public MainWindow()
        {
            #region UI

            InitializeComponent();

            if (Settings.Default.IsStartMinimized)
            {
                Visibility = Visibility.Hidden;
            }

            InputTextBox.Focus();

            #endregion

            #region Hook

            Hook.KeyUpEvent += Global_KeyUp;
            Hook.KeyDownEvent += Global_KeyDown;

            #endregion

            #region Global Runner

            byte[] alternativeConverterLastData = null;
            GlobalRunner.NotHandledText += async _ =>
            {
                if (alternativeConverterLastData == Manager.Data)
                {
                    return;
                }
                alternativeConverterLastData = Manager.Data;

                if (Manager.Data == null)
                {
                    return;
                }
                var text = await AlternativeConverter.Convert(Manager.Data);
                GlobalRunner.Run(text, null);
            };
            GlobalRunner.NewOutput += (o, args) => Print(args.Text);
            GlobalRunner.NewSpeech += (o, args) => Say(args.Text);
            GlobalRunner.NewCommand += (o, args) => Manager.ProcessText(args.Text);

            #endregion

            #region Manager

            Manager.NewText += text => GlobalRunner.Run(text, null);
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

            #endregion

            #region FileSystemWatcher

            Watcher.EnableRaisingEvents = true;
            Watcher.IncludeSubdirectories = true;
            Watcher.Created += async (sender, args) =>
            {
                try
                {
                    await Task.Delay(100);

                    var command = File.ReadAllText(args.FullPath);

                    GlobalRunner.Run(command.Trim(), null);

                    //File.Delete(args.FullPath);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
            };

            #endregion

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

            AlternativeConverter?.Dispose();
            AlternativeConverter = null;

            Manager?.Dispose();
            Manager = null;

            GlobalRunner?.Dispose();
            GlobalRunner = null;

            Synthesizer?.Dispose();
            Synthesizer = null;
        }

        #endregion

        #region Private methods

        private void Print(string text) => Dispatcher.Invoke(() => ConsoleTextBox.Text += $"{DateTime.Now:T}: {text}{Environment.NewLine}");

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

                    GlobalRunner.Run(InputTextBox.Text, null);
                    InputTextBox.Clear();
                    break;

                case Key.Up:
                    if (GlobalRunner.History.Any())
                    {
                        InputTextBox.Text = GlobalRunner.History.LastOrDefault() ?? "";
                        GlobalRunner.History.RemoveAt(GlobalRunner.History.Count - 1);
                    }
                    break;
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e) => Manager.ChangeWithTimeout(3000);

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CommandsWindow(GlobalRunner.Storage);

            window.ShowDialog();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();

            window.ShowDialog();
        }

        private void Global_KeyUp(KeyboardHookEventArgs e)
        {
            if ((int)e.Key == 192 || e.isAltPressed && e.isCtrlPressed)
            {
                Manager.Stop();
            }
        }

        private void Global_KeyDown(KeyboardHookEventArgs e)
        {
            if ((int)e.Key == 192 || e.isAltPressed && e.isCtrlPressed)
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