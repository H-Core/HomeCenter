using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using H.NET.Core;
using H.NET.Core.Runners;
using H.NET.Plugins;
using H.NET.Utilities;
using HomeCenter.NET.Extensions;
using HomeCenter.NET.Services;
using HomeCenter.NET.Utilities;
using HomeCenter.NET.ViewModels.Commands;
using HomeCenter.NET.ViewModels.Modules;
using HomeCenter.NET.ViewModels.Settings;

namespace HomeCenter.NET.ViewModels
{
    internal class MainViewModel : Screen, IDisposable
    {
        #region Properties

        public IWindowManager WindowManager { get; }
        public MainService MainService { get; }
        public HookService HookService { get; }
        public Properties.Settings Settings { get; }
        public PopUpViewModel PopUpViewModel { get; }

        private string _text;
        public string Text {
            get => _text;
            set {
                _text = value;
                NotifyOfPropertyChange(nameof(Text));
            }
        }

        private string _input;
        public string Input {
            get => _input;
            set {
                _input = value;
                NotifyOfPropertyChange(nameof(Input));
            }
        }

        private bool _isVisible;
        public bool IsVisible {
            get => _isVisible;
            set {
                _isVisible = value;
                NotifyOfPropertyChange(nameof(IsVisible));
            }
        }

        private bool _isRecord;
        public bool IsRecord {
            get => _isRecord;
            set {
                _isRecord = value;
                NotifyOfPropertyChange(nameof(IsRecord));
            }
        }

        private bool UserCanClose { get; set; }

        #endregion

        #region Constructors

        public MainViewModel(IWindowManager windowManager, Properties.Settings settings, PopUpViewModel popUpViewModel, MainService mainService, HookService hookService)
        {
            WindowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            PopUpViewModel = popUpViewModel ?? throw new ArgumentNullException(nameof(popUpViewModel));
            MainService = mainService ?? throw new ArgumentNullException(nameof(mainService));
            HookService = hookService ?? throw new ArgumentNullException(nameof(hookService));

            MainService.GlobalRunner.NewOutput += Print;
            MainService.Manager.Started += (sender, args) =>
            {
                IsRecord = true;
                MainService.HiddenRun("deskband start");
            };
            MainService.Manager.Stopped += (sender, args) =>
            {
                IsRecord = false;
                MainService.HiddenRun("deskband stop");
            };
        }

        #endregion

        #region Public methods

        public void Print(string text)
        {
            Text += $"{DateTime.Now:T}: {text}{Environment.NewLine}";

            if (Settings.EnablePopUpMessages)
            {
                PopUpViewModel.Show(text, 5000);
            }
        }

        public async Task Say(string text)
        {
            var synthesizer = Options.Synthesizer;
            if (synthesizer == null)
            {
                Print("Synthesizer is not found");
                return;
            }

            var bytes = await synthesizer.Convert(text);

            await bytes.PlayAsync();
        }

        public async Task<List<string>> Search(string text)
        {
            var searcher = Options.Searcher;
            if (searcher == null)
            {
                Print("Searcher is not found");
                return new List<string>();
            }

            return await searcher.Search(text);
        }

        public void Record()
        {
            MainService.StartRecord(3000);
        }

        public void ShowCommands()
        {
            this.ShowWindow<CommandsViewModel>();
        }

        public void ShowSettings()
        {
            this.ShowWindow<SettingsViewModel>();
        }

        public void ShowModuleSettings(string name)
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

            this.ShowWindow(new ModuleSettingsViewModel(module));
        }

        public async Task Load(bool isUpdating = false)
        {
            #region Modules

            AssembliesManager.LogAction = Print;
            Module.LogAction = Print;
            Runner.SearchFunc = Search;

            Print("Loading modules...");
            try
            {
                await MainService.Load();

                HookService.KeyboardHook.SetEnabled(Settings.EnableKeyboardHook);
                HookService.MouseHook.SetEnabled(Settings.EnableMouseHook);

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
                HookService.KeyboardHook.KeyUp += Global_KeyUp;
                HookService.KeyboardHook.KeyDown += Global_KeyDown;

                //ScreenshotRectangle.ActivationKeys.Add(Key.Space);
                //ScreenshotRectangle.ActivationModifiers.Add(ModifierKeys.Shift);
                //ScreenshotRectangle.NewImage += image => Clipboard.SetImage(image.ToBitmapImage());
                //HookService.MouseHook.MouseUp += ScreenshotRectangle.Global_MouseUp;
                //HookService.MouseHook.MouseDown += ScreenshotRectangle.Global_MouseDown;
                //HookService.MouseHook.MouseMove += ScreenshotRectangle.Global_MouseMove;

                HookService.MouseHook.MouseDown += Global_MouseDown;
            }
            catch (Exception exception)
            {
                Print(exception.ToString());
            }

            #endregion
        }

        public void Input_KeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (Input.Length == 0)
                    {
                        break;
                    }
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        Input += Environment.NewLine;
                        //InputTextBox.CaretIndex = InputTextBox.Text.Length - 1;
                        break;
                    }

                    MainService.Run(Input);
                    Input = string.Empty;
                    break;

                case Key.Up:
                    if (MainService.GlobalRunner.History.Any())
                    {
                        Input = MainService.GlobalRunner.History.LastOrDefault() ?? "";
                        MainService.GlobalRunner.History.RemoveAt(MainService.GlobalRunner.History.Count - 1);
                    }
                    break;
            }
        }

        private void Global_KeyUp(object sender, KeyboardHookEventArgs e)
        {
            if (e.Key != Keys.None && e.Key == Options.RecordKey ||
                e.IsAltPressed && e.IsCtrlPressed)
            {
                MainService.Manager.Stop();
            }
        }

        private void Global_KeyDown(object sender, KeyboardHookEventArgs e)
        {
            if (e.Key != Keys.None && e.Key == Options.RecordKey ||
                e.Key == Keys.Space && e.IsAltPressed && e.IsCtrlPressed)
            {
                MainService.Manager.Start();
            }

            if (Options.IsIgnoredApplication())
            {
                return;
            }

            //Print($"{e.Key:G}");
            var combination = new KeysCombination(e.Key, e.IsCtrlPressed, e.IsShiftPressed, e.IsAltPressed);
            if (MainService.RunCombination(combination))
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
            if (Options.IsIgnoredApplication())
            {
                return;
            }

            var combination = KeysCombination.FromSpecialData(e.SpecialButton);
            if (MainService.RunCombination(combination))
            {
                e.Handled = true;
            }
            //Print($"{e.SpecialButton}");
        }

        public void OnKeyUp(KeyEventArgs e)
        {
            // TODO: to record key
            if (e.Key == Key.OemTilde)
            {
                e.Handled = true;
            }
        }

        public void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.OemTilde)
            {
                e.Handled = true;
            }

            // TODO: To global?
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.R))
            {
                e.Handled = true;

                MainService.Restart();
            }
        }

        public void Close()
        {
            UserCanClose = true;
            TryClose();
        }

        public void Dispose()
        {
            /* TODO: fix
            if (TaskbarIcon != null)
            {
                TaskbarIcon.Icon = null;
                TaskbarIcon.Visibility = Visibility.Hidden;
                TaskbarIcon.Dispose();
                TaskbarIcon = null;
            }*/
        }

        #endregion

        #region Event Handlers

        public void OnClosing(CancelEventArgs e)
        {
            if (UserCanClose)
            {
                PopUpViewModel.TryClose();
                return;
            }

            IsVisible = false;
            e.Cancel = true;
        }

        #endregion
    }
}
