using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using H.NET.Core;
using H.NET.Core.Managers;
using H.NET.Core.Notifiers;
using H.NET.Core.Recorders;
using H.NET.Plugins;
using H.NET.Storages;
using H.NET.Storages.Extensions;
using H.NET.Utilities;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Runners;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Windows
{
    public partial class MainWindow : IDisposable
    {
        #region Properties

        private BaseManager Manager { get; set; } = new BaseManager();
        private ISynthesizer Synthesizer { get; set; }

        private Server Server { get; } = new Server(Options.IpcPortToHomeCenter);

        private static LowLevelMouseHook MouseHook { get; set; } = new LowLevelMouseHook();
        private static LowLevelKeyboardHook KeyboardHook { get; set; } = new LowLevelKeyboardHook();

        private GlobalRunner GlobalRunner { get; set; } = new GlobalRunner(new CommandsStorage(Options.CompanyName));

        private bool CanClose { get; set; }

        private bool DeskBandRecordStarted { get; set; }

        private static Action<string> GlobalRunAction { get; set; }
        public static void GlobalRun(string text) => GlobalRunAction?.Invoke(text);

        private Dictionary<KeysCombination, Command> Combinations { get; } = new Dictionary<KeysCombination, Command>();

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

            #region Global Runner

            GlobalRunner.NewOutput += Print;
            GlobalRunner.NewSpeech += Say;
            GlobalRunner.NewCommand += Manager.ProcessText;

            #endregion

            #region Manager

            Manager.NewText += Run;
            Manager.Started += (sender, args) => Dispatcher.Invoke(() =>
            {
                RecordButton.Content = "🔊";
                RecordButton.Background = Brushes.LightSkyBlue;

                if (!DeskBandRecordStarted)
                {
                    Run("deskband start");
                    DeskBandRecordStarted = true;
                }
            });
            Manager.Stopped += (sender, args) => Dispatcher.Invoke(() =>
            {
                RecordButton.Content = "🔉";
                RecordButton.Background = Brushes.LightGray;

                if (DeskBandRecordStarted)
                {
                    Run("deskband stop");
                    DeskBandRecordStarted = false;
                }
            });

            #endregion

            Server.NewMessage += Run;

            #region Default Runner

            DefaultRunner.ShowSettingsAction = () => Dispatcher.Invoke(() => SettingsButton_Click(this, EventArgs.Empty));
            DefaultRunner.ShowCommandsAction = () => Dispatcher.Invoke(() => MenuButton_Click(this, EventArgs.Empty));
            DefaultRunner.StartRecordAction = () => Dispatcher.Invoke(() => RecordButton_Click(this, EventArgs.Empty));
            DefaultRunner.ClipboardAction = command => Dispatcher.Invoke(() => Clipboard.SetText(command));
            DefaultRunner.ClipboardFunc = () => Dispatcher.Invoke(Clipboard.GetText);

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

            Manager?.Dispose();
            Manager = null;

            GlobalRunner?.Dispose();
            GlobalRunner = null;

            KeyboardHook?.Dispose();
            KeyboardHook = null;

            MouseHook?.Dispose();
            MouseHook = null;
        }

        #endregion

        #region Private methods

        private void Print(string text) => Dispatcher.Invoke(() => ConsoleTextBox.Text += $"{DateTime.Now:T}: {text}{Environment.NewLine}");

        private static void Say(byte[] bytes) => bytes?.Play();
        private async void Say(string text)
        {
            if (Synthesizer == null)
            {
                Print("Synthesizer is not found");
                return;
            }

            Say(await Synthesizer.Convert(text));
        } 

        private async void Run(string message) => await GlobalRunner.Run(message);

        #endregion

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Hook

            try
            {
                KeyboardHook.Start();
                MouseHook.Start();

                KeyboardHook.KeyUp += Global_KeyUp;
                KeyboardHook.KeyDown += Global_KeyDown;

                MouseHook.MouseDown += Global_MouseDown;
            }
            catch (Exception exception)
            {
                Print(exception.ToString());
            }

            #endregion

            #region Modules

            AssembliesManager.LogAction = Print;
            Module.LogAction = Print;
            Notifier.RunAction = Run;
            GlobalRunAction = Run;

            Print("Loading modules...");
            try
            {
                ModuleManager.Instance.Load();
                ModuleManager.AddUniqueInstancesIfNeed();
                ModuleManager.RegisterHandlers(Print, Say, Run);

                SetUpRuntimeModule();

                Print("Loaded");
            }
            catch (Exception exception)
            {
                Print(exception.ToString());
            }

            #endregion
        }

        private void InputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (InputTextBox.Text.Length == 0)
                    {
                        break;
                    }
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        InputTextBox.Text += Environment.NewLine;
                        InputTextBox.CaretIndex = InputTextBox.Text.Length - 1;
                        break;
                    }
                    
                    Run(InputTextBox.Text);
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

        private void RecordButton_Click(object sender, EventArgs e)
        {
            Manager.ChangeWithTimeout(3000);
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            var window = new CommandsWindow(GlobalRunner);

            window.Show();
            window.Closed += (o, args) => SetUpRuntimeModule();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            var window = new SettingsWindow();

            window.Show();
            window.Closed += (o, args) => SetUpRuntimeModule();
        }

        private void SetUpRuntimeModule()
        {
            Manager.Recorder = Options.Recorder;
            Manager.Converter = Options.Converter;
            Manager.AlternativeConverters = Options.AlternativeConverters;
            Synthesizer = Options.Synthesizer;

            Combinations.Clear();
            foreach (var pair in GlobalRunner.Storage.UniqueValues(i => i.Value).Where(i => i.Value.HotKey != null))
            {
                var command = pair.Value;
                var hotKey = command.HotKey;
                var combination = KeysCombination.FromString(hotKey);
                if (combination.IsEmpty)
                {
                    continue;
                }

                Combinations[combination] = command;
            }
        }

        private void Global_KeyUp(object sender, KeyboardHookEventArgs e)
        {
            if (e.Key == Options.RecordKey || e.IsAltPressed && e.IsCtrlPressed)
            {
                Manager.Stop();
            }
        }

        private bool FindCombinationAndRun(KeysCombination combination)
        {
            if (!Combinations.TryGetValue(combination, out var command))
            {
                return false;
            }

            Run(command.Keys.FirstOrDefault()?.Text);
            return true;
        }

        private void Global_KeyDown(object sender, KeyboardHookEventArgs e)
        {
            if (e.Key == Options.RecordKey || e.IsAltPressed && e.IsCtrlPressed)
            {
                Manager.Start();
            }

            //Print($"{e.Key:G}");
            var combination = new KeysCombination(e.Key, e.IsCtrlPressed, e.IsShiftPressed, e.IsAltPressed);
            if (FindCombinationAndRun(combination))
            {
                e.Handled = true;
            }
        }

        private void Global_MouseDown(object sender, MouseEventExtArgs e)
        {
            if (e.SpecialButton == 0)
            {
                return;
            }

            var combination = KeysCombination.FromSpecialData(e.SpecialButton);
            if (FindCombinationAndRun(combination))
            {
                e.Handled = true;
            }
            //Print($"{e.SpecialButton}");
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

        #region Static methods

        public static async Task<KeysCombination> CatchKey()
        {
            if (KeyboardHook == null || MouseHook == null)
            {
                return null;
            }

            // Starts if not started
            KeyboardHook.Start();
            MouseHook.Start();

            KeysCombination combination = null;
            var isCancel = false;

            void OnKeyboardHookOnKeyDown(object sender, KeyboardHookEventArgs args)
            {
                args.Handled = true;
                if (args.Key == Keys.Escape)
                {
                    isCancel = true;
                    return;
                }

                combination = new KeysCombination(args.Key, args.IsCtrlPressed, args.IsShiftPressed, args.IsAltPressed);
            }

            void OnMouseHookOnMouseDown(object sender, MouseEventExtArgs args)
            {
                if (args.SpecialButton == 0)
                {
                    return;
                }

                args.Handled = true;
                combination = KeysCombination.FromSpecialData(args.SpecialButton);
            }

            KeyboardHook.KeyDown += OnKeyboardHookOnKeyDown;
            MouseHook.MouseDown += OnMouseHookOnMouseDown;

            while (!isCancel && (combination == null || combination.IsEmpty))
            {
                await Task.Delay(1);
            }

            KeyboardHook.KeyDown -= OnKeyboardHookOnKeyDown;
            MouseHook.MouseDown -= OnMouseHookOnMouseDown;

            return isCancel ? null : combination;
        }

        #endregion
    }
}