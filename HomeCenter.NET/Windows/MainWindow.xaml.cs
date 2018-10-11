using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using H.NET.Core;
using H.NET.Core.Managers;
using H.NET.Core.Recorders;
using H.NET.Core.Runners;
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
        private ISearcher Searcher { get; set; }

        private IpcServer IpcServer { get; } = new IpcServer(Options.IpcPortToHomeCenter);

        private static LowLevelMouseHook MouseHook { get; set; } = new LowLevelMouseHook();
        private static LowLevelKeyboardHook KeyboardHook { get; set; } = new LowLevelKeyboardHook();

        private GlobalRunner GlobalRunner { get; set; } = new GlobalRunner(new CommandsStorage(Options.CompanyName));

        private bool CanClose { get; set; }

        private static Action<string> GlobalRunAction { get; set; }
        public static void GlobalRun(string text) => GlobalRunAction?.Invoke(text);

        private Dictionary<KeysCombination, Command> Combinations { get; } = new Dictionary<KeysCombination, Command>();

        private SettingsWindow SettingsWindow { get; set; }
        private CommandsWindow CommandsWindow { get; set; }

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            #region Global Runner

            GlobalRunner.NewOutput += Print;

            #endregion

            #region Manager

            Manager.NewText += text =>
            {
                if (Runner.IsWaitCommand)
                {
                    Runner.StopWaitCommand(text);
                    return;
                }

                Run(text);
            };
            Manager.Started += (sender, args) => Dispatcher.Invoke(() =>
            {
                RecordButton.Content = "🔊";
                RecordButton.Background = Brushes.LightSkyBlue;

                HiddenRun("deskband start");
            });
            Manager.Stopped += (sender, args) => Dispatcher.Invoke(() =>
            {
                RecordButton.Content = "🔉";
                RecordButton.Background = Brushes.LightGray;

                HiddenRun("deskband stop");
            });

            #endregion

            IpcServer.NewMessage += Run;
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

            KeyboardHook?.Dispose();
            KeyboardHook = null;

            MouseHook?.Dispose();
            MouseHook = null;
        }

        #endregion

        #region Private methods

        private void Print(string text) => Dispatcher.Invoke(() =>
        {
            ConsoleTextBox.Text += $"{DateTime.Now:T}: {text}{Environment.NewLine}";

            if (Settings.Default.EnablePopUpMessages)
            {
                PopUpWindow.Show(text, 5000);
            }
        });

        private async Task Say(string text)
        {
            if (Synthesizer == null)
            {
                Print("Synthesizer is not found");
                return;
            }

            var bytes = await Synthesizer.Convert(text);

            await bytes.PlayAsync();
        }

        private async Task<List<string>> Search(string text)
        {
            if (Searcher == null)
            {
                Print("Searcher is not found");
                return new List<string>();
            }

            return await Searcher.Search(text);
        }

        private async Task HiddenRunAsync(string message) => await GlobalRunner.Run(message, false);

        private async void Run(string message) => await GlobalRunner.Run(message);
        private async void HiddenRun(string message) => await GlobalRunner.Run(message, false);

        private void SetUpRuntimeModule()
        {
            KeyboardHook.SetEnabled(Settings.Default.EnableKeyboardHook);
            MouseHook.SetEnabled(Settings.Default.EnableMouseHook);

            Manager.Recorder = Options.Recorder;
            Manager.Converter = Options.Converter;
            Manager.AlternativeConverters = Options.AlternativeConverters;
            Synthesizer = Options.Synthesizer;
            Searcher = Options.Searcher;

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

        private void ShowCommands()
        {
            if (CommandsWindow != null && CommandsWindow.IsLoaded)
            {
                CommandsWindow.Show();
                return;
            }

            CommandsWindow = new CommandsWindow(GlobalRunner);
            CommandsWindow.Closed += (o, args) =>
            {
                SetUpRuntimeModule();
                CommandsWindow = null;
            };

            CommandsWindow.Show();
        }

        private void ShowSettings()
        {
            if (SettingsWindow != null && SettingsWindow.IsLoaded)
            {
                SettingsWindow.Show();
                return;
            }

            SettingsWindow = new SettingsWindow();
            SettingsWindow.Closed += (o, args) =>
            {
                SetUpRuntimeModule();
                SettingsWindow = null;
            };

            SettingsWindow.Show();
        }

        private void ShowModuleSettings(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Print("ShowModuleSettings: Module name is empty");
                return;
            }

            var module = ModuleManager.Instance.GetPlugin<IModule>(name)?.Value;
            if (module == null)
            {
                Print($"ShowModuleSettings: Module {name} is not found");
                return;
            }

            var window = new ModuleSettingsWindow(module);
            window.Show();

        }

        #endregion

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Focus();
        }

        public async Task Load(bool isUpdating = false)
        {
            #region Static Runners

            var staticRunners = new List<IRunner>
            {
                new DefaultRunner(Print, Say, Search),
                new KeyboardRunner(),
                new WindowsRunner(),
                new ClipboardRunner
                {
                    ClipboardAction = command => Dispatcher.Invoke(() => Clipboard.SetText(command)),
                    ClipboardFunc = () => Dispatcher.Invoke(Clipboard.GetText)
                },
                new UiRunner
                {
                    RestartAction = command => Dispatcher.Invoke(() => Restart(command)),
                    UpdateRestartAction = command => Dispatcher.Invoke(() => UpdateRestart(command)),
                    ShowUiAction = () => Dispatcher.Invoke(Show),
                    ShowSettingsAction = () => Dispatcher.Invoke(ShowSettings),
                    ShowCommandsAction = () => Dispatcher.Invoke(ShowCommands),
                    ShowModuleSettingsAction = name => Dispatcher.Invoke(() => ShowModuleSettings(name)),
                    StartRecordAction = timeout => Dispatcher.Invoke(() => StartRecord(timeout))
                }
            };
            foreach (var runner in staticRunners)
            {
                ModuleManager.Instance.AddStaticInstance(runner.ShortName, runner);
            }

            #endregion

            #region Modules

            AssembliesManager.LogAction = Print;
            Module.LogAction = Print;
            Runner.GetVariableValueGlobalFunc = GlobalRunner.GetVariableValue;
            Runner.SearchFunc = Search;
            GlobalRunAction = Run;

            Print("Loading modules...");
            try
            {
                ModuleManager.RunAction = Run; // TODO: Hidden?
                ModuleManager.RunAsyncFunc = HiddenRunAsync;
                await Task.Run(() =>
                {
                    ModuleManager.Instance.Load();
                    ModuleManager.Instance.EnableInstances();
                });
                ModuleManager.AddUniqueInstancesIfNeed();
                ModuleManager.RegisterHandlers();

                SetUpRuntimeModule();

                Print("Loaded");
            }
            catch (Exception exception)
            {
                Print(exception.ToString());
            }

            #endregion

            #region Hook

            try
            {
                KeyboardHook.KeyUp += Global_KeyUp;
                KeyboardHook.KeyDown += Global_KeyDown;

                ScreenshotRectangle.ActivationKeys.Add(Key.Space);
                ScreenshotRectangle.ActivationModifiers.Add(ModifierKeys.Shift);
                MouseHook.MouseUp += ScreenshotRectangle.Global_MouseUp;
                MouseHook.MouseDown += ScreenshotRectangle.Global_MouseDown;
                MouseHook.MouseMove += ScreenshotRectangle.Global_MouseMove;

                MouseHook.MouseDown += Global_MouseDown;
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

        private void RecordButton_Click(object sender, EventArgs e) => StartRecord(3000);

        private void MenuButton_Click(object sender, EventArgs e) => ShowCommands();
        private void SettingsButton_Click(object sender, EventArgs e) => ShowSettings();

        // TODO: Fix multi key up
        private void Global_KeyUp(object sender, KeyboardHookEventArgs e)
        {
            if (e.Key != Keys.None && e.Key == Options.RecordKey ||
                e.IsAltPressed && e.IsCtrlPressed)
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
            if (e.Key != Keys.None && e.Key == Options.RecordKey ||
                e.Key == Keys.Space && e.IsAltPressed && e.IsCtrlPressed)
            {
                Manager.Start();
            }

            if (Options.IsIgnoredApplication())
            {
                return;
            }

            //Print($"{e.Key:G}");
            var combination = new KeysCombination(e.Key, e.IsCtrlPressed, e.IsShiftPressed, e.IsAltPressed);
            if (FindCombinationAndRun(combination))
            {
                e.Handled = true;
            }
        }

        #region Global Mouse
        
        private void Global_MouseDown(object sender, MouseEventExtArgs e)
        {
            if (e.SpecialButton == 0)
            {
                return;
            }
            if (Options.IsIgnoredApplication())
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

        #endregion

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

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.R))
            {
                e.Handled = true;

                Restart();
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

        private void UpdateRestart(string command) => Restart(command, "/updating");

        private void Restart() => Restart(new List<string>());
        private void Restart(string command, string additionalArguments = null) => Restart(new[] {command}, additionalArguments);

        private void Restart(ICollection<string> commands, string additionalArguments = null)
        {
            var run = commands.Any() ? $"/run \"{string.Join(";", commands)}\"" : string.Empty;

            IpcServer.Stop();
            Process.Start($"\"{Options.FilePath}\"", $"/restart {run} {additionalArguments}");
            Application.Current.Shutdown();
        }

        private void StartRecord(int timeout)
        {
            Manager.ChangeWithTimeout(timeout);
        }

        public static async Task<KeysCombination> CatchKey()
        {
            if (KeyboardHook == null || MouseHook == null)
            {
                return null;
            }

            var keyboardHookState = KeyboardHook.IsStarted;
            var mouseHookState = MouseHook.IsStarted;

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

            KeyboardHook.SetEnabled(keyboardHookState);
            MouseHook.SetEnabled(mouseHookState);

            return isCancel ? null : combination;
        }

        #endregion
    }
}