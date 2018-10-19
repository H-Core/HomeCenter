using System;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using H.NET.Core.Managers;
using H.NET.Core.Recorders;
using HomeCenter.NET.Extensions;
using HomeCenter.NET.Services;
using HomeCenter.NET.ViewModels.Commands;
using HomeCenter.NET.ViewModels.Settings;

namespace HomeCenter.NET.ViewModels
{
    public class MainViewModel : Screen
    {
        #region Properties

        public IWindowManager WindowManager { get; }
        public RunnerService RunnerService { get; }
        public HookService HookService { get; }
        public Properties.Settings Settings { get; }
        public PopUpViewModel PopUpViewModel { get; }
        public BaseManager Manager { get; }

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

        public MainViewModel(IWindowManager windowManager, Properties.Settings settings, PopUpViewModel popUpViewModel, RunnerService runnerService, HookService hookService, BaseManager manager)
        {
            WindowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            PopUpViewModel = popUpViewModel ?? throw new ArgumentNullException(nameof(popUpViewModel));
            RunnerService = runnerService ?? throw new ArgumentNullException(nameof(runnerService));
            HookService = hookService ?? throw new ArgumentNullException(nameof(hookService));
            Manager = manager ?? throw new ArgumentNullException(nameof(manager));

            RunnerService.NewOutput += Print;
            Manager.Started += (sender, args) =>
            {
                IsRecord = true;
                RunnerService.HiddenRun("deskband start");
            };
            Manager.Stopped += (sender, args) =>
            {
                IsRecord = false;
                RunnerService.HiddenRun("deskband stop");
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

        public void Record()
        {
            Manager.ChangeWithTimeout(3000);
        }

        public void ShowCommands()
        {
            this.ShowWindow<CommandsViewModel>();
        }

        public void ShowSettings()
        {
            this.ShowWindow<SettingsViewModel>();
        }

        public void BeforeExit()
        {
            PopUpViewModel.TryClose();
        }

        public void PreviousCommand()
        {
            if (!RunnerService.History.Any())
            {
                return;
            }

            Input = RunnerService.History.LastOrDefault() ?? "";
            RunnerService.History.RemoveAt(RunnerService.History.Count - 1);
        }

        public void RunInput()
        {
            if (string.IsNullOrEmpty(Input))
            {
                return;
            }

            RunnerService.Run(Input);
            Input = string.Empty;
        }

        public void AddNewLine()
        {
            Input += Environment.NewLine;
            // TODO: Need to implement
            //InputTextBox.CaretIndex = InputTextBox.Text.Length - 1;
        }

        public void Restart()
        {
            RunnerService.Run("restart");
        }

        public void OnClosing(CancelEventArgs e)
        {
            if (UserCanClose)
            {
                BeforeExit();
                return;
            }

            IsVisible = false;
            e.Cancel = true;
        }

        public void Close()
        {
            UserCanClose = true;
            TryClose();

            if (!IsVisible)
            {
                BeforeExit();
            }
        }

        #endregion
    }
}
