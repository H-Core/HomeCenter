using System;
using System.Linq;
using Caliburn.Micro;
using H.NET.Storages;
using HomeCenter.NET.Windows;
// ReSharper disable UnusedMember.Global

namespace HomeCenter.NET.ViewModels
{
    public class CommandSettingsViewModel : Screen
    {
        /*
        #region Static methods

        public static (bool isSaved, Command newCommand) Show(Command command)
        {
            var window = new ChangeCommandWindow(command);
            var result = window.ShowDialog() == true;

            return (result, window.Command);
        }

        public static bool ShowAndSaveIfNeeded(Command command, IStorage<Command> storage)
        {
            var (result, newCommand) = Show(command);
            if (!result)
            {
                return false;
            }

            foreach (var key in command.Keys)
            {
                storage.Remove(key.Text);
            }

            foreach (var key in newCommand.Keys)
            {
                storage[key.Text] = newCommand;
            }

            return true;
        }
        #endregion
        
        */
        #region Properties

        public Command Command { get; }
        public BindableCollection<SingleKeyViewModel> Keys { get; }
        public BindableCollection<SingleCommandViewModel> Commands { get; }

        public string HotKey
        {
            get => Command.HotKey;
            set
            {
                Command.HotKey = value;
                NotifyOfPropertyChange(nameof(HotKey));
            }
        }

        private bool _editHotKeyIsEnabled = true;
        public bool EditHotKeyIsEnabled {
            get => _editHotKeyIsEnabled;
            set {
                _editHotKeyIsEnabled = value;
                NotifyOfPropertyChange(nameof(EditHotKeyIsEnabled));
            }
        }

        #endregion

        #region Constructors

        public CommandSettingsViewModel(Command command)
        {
            Command = command?.Clone() as Command ?? throw new ArgumentNullException(nameof(command));

            Keys = new BindableCollection<SingleKeyViewModel>(Command.Keys.Select(CreateKeyModel));
            Commands = new BindableCollection<SingleCommandViewModel>(Command.Lines.Select(CreateCommandModel));
            HotKey = command.HotKey;
        }

        #endregion

        #region Factories

        public static SingleKeyViewModel CreateKeyModel(SingleKey key) =>
            new SingleKeyViewModel(key);

        public static SingleCommandViewModel CreateCommandModel(SingleCommand command) => 
            new SingleCommandViewModel(command, null, command.Text, editable: true, run: true, delete: true);

        #endregion

        #region Public methods

        #region Key methods

        public void AddKey()
        {
            var key = new SingleKey(string.Empty);

            Command.Keys.Add(key);
            Keys.Add(CreateKeyModel(key));

            /* TODO: Focus last element
            var control = DataPanel.Children.OfType<TextControl>().LastOrDefault();
            if (control == null)
            {
                return;
            }

            control.TextBox.Focusable = true;
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(control.TextBox), control.TextBox);
            Keyboard.Focus(control.TextBox);
            control.TextBox.Focus();*/
        }

        public void DeleteKey(SingleKeyViewModel viewModel)
        {
            Keys.Remove(viewModel);
            Command.Keys.Remove(viewModel.Key);
        }

        #endregion

        #region Command methods

        public void DeleteCommand(SingleCommandViewModel viewModel)
        {
            Commands.Remove(viewModel);
            Command.Lines.Remove(viewModel.Command);
        }

        public void RunCommand(SingleCommandViewModel viewModel)
        {
            MainWindow.GlobalRun(viewModel.Description);
        }

        public void AddCommand()
        {
            var command = new SingleCommand(string.Empty);

            Command.Lines.Add(command);
            Commands.Add(CreateCommandModel(command));
        }

        #endregion

        #region HotKey methods

        public async void EditHotKey()
        {
            EditHotKeyIsEnabled = false;

            var combination = await MainWindow.CatchKey();
            HotKey = combination?.ToString() ?? string.Empty;

            EditHotKeyIsEnabled = true;
        }

        #endregion

        #endregion

    }
}
