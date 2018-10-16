using System;
using System.Linq;
using Caliburn.Micro;
using H.NET.Storages;
using H.NET.Storages.Extensions;
using HomeCenter.NET.Runners;
using HomeCenter.NET.Windows;

// ReSharper disable UnusedMember.Global

namespace HomeCenter.NET.ViewModels.Commands
{
    public class CommandsViewModel : SaveCancelViewModel
    {
        #region Properties

        public IWindowManager Manager { get; }
        public GlobalRunner Runner { get; }
        public BindableCollection<UserCommandViewModel> UserCommands { get; }
        public BindableCollection<AllCommandViewModel> AllCommands { get; }
        public BindableCollection<VariableViewModel> Variables { get; }
        public BindableCollection<ProcessViewModel> Processes { get; }

        #endregion

        #region Constructors

        public CommandsViewModel(GlobalRunner runner, IWindowManager manager)
        {
            Runner = runner ?? throw new ArgumentNullException(nameof(runner));
            Manager = manager ?? throw new ArgumentNullException(nameof(manager));

            UserCommands = new BindableCollection<UserCommandViewModel>(
                Runner.Storage.UniqueValues(entry => entry.Value).Select(i => new UserCommandViewModel(i.Value)));
            AllCommands = new BindableCollection<AllCommandViewModel>(
                Runner.GetSupportedCommands().Select(i => new AllCommandViewModel(i)));
            Variables = new BindableCollection<VariableViewModel>(
                Runner.GetSupportedVariables().Select(i => new VariableViewModel(i)));
            Processes = new BindableCollection<ProcessViewModel>(
                Runner.Processes.Select(i => new ProcessViewModel(i)));

            // TODO: May be -event required?
            Runner.BeforeRun += (o, e) => NotifyOfPropertyChange(nameof(Processes));
            Runner.AfterRun += (o, e) => NotifyOfPropertyChange(nameof(Processes));

            SaveAction = () => Runner.Storage.Save();
            CancelAction = () => Runner.Storage.Load(); // Cancel changes TODO: may me need to use TempStorage instead this?
        }

        #endregion

        #region Public methods

        public void Add()
        {
            var command = new Command();
            command.Keys.Add(new SingleKey(string.Empty));
            command.Lines.Add(new SingleCommand(string.Empty));

            var viewModel = new CommandSettingsViewModel(command);
            var result = Manager.ShowDialog(viewModel);
            if (result != true)
            {
                return;
            }

            UserCommands.Add(new UserCommandViewModel(command));
            foreach (var key in command.Keys)
            {
                Runner.Storage[key.Text] = command;
            }
        }

        public void EditCommand(CommandViewModel viewModel)
        {
            switch (viewModel)
            {
                case UserCommandViewModel userCommandViewModel:
                    var newCommand = (Command)userCommandViewModel.Command.Clone();
                    var dialogViewModel = new CommandSettingsViewModel(newCommand);
                    var result = Manager.ShowDialog(dialogViewModel);
                    if (result != true)
                    {
                        return;
                    }

                    foreach (var key in userCommandViewModel.Command.Keys)
                    {
                        Runner.Storage.Remove(key.Text);
                    }
                    foreach (var key in newCommand.Keys)
                    {
                        Runner.Storage[key.Text] = newCommand;
                    }

                    var index = UserCommands.IndexOf(userCommandViewModel);
                    UserCommands.Remove(userCommandViewModel);
                    UserCommands.Insert(index, new UserCommandViewModel(newCommand));

                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public void DeleteCommand(CommandViewModel viewModel)
        {
            switch (viewModel)
            {
                case UserCommandViewModel userCommandViewModel:
                    foreach (var key in userCommandViewModel.Command.Keys)
                    {
                        Runner.Storage.Remove(key.Text);
                    }
                    UserCommands.Remove(userCommandViewModel);
                    break;

                case ProcessViewModel processViewModel:
                    processViewModel.Process.Cancel();
                    NotifyOfPropertyChange(nameof(Processes));
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public void RunCommand(CommandViewModel viewModel)
        {
            switch (viewModel)
            {
                case UserCommandViewModel userCommandViewModel:
                    foreach (var line in userCommandViewModel.Command.Lines)
                    {
                        MainWindow.GlobalRun(line.Text);
                    }
                    break;

                case AllCommandViewModel allCommandViewModel:
                    MainWindow.GlobalRun($"{allCommandViewModel.Prefix} {viewModel.Description}");
                    break;

                case VariableViewModel _:
                    viewModel.Description = Runner.GetVariableValue(viewModel.Name)?.ToString() ?? string.Empty;
                    break;

                case ProcessViewModel _:
                    MainWindow.GlobalRun(viewModel.Name);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
        
        #endregion

    }
}
