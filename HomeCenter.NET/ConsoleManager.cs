using System;
using System.Linq;
using HomeCenter.NET.Utilities;
using VoiceActions.NET;
using VoiceActions.NET.Runners.Core;
using VoiceActions.NET.Utilities;

namespace HomeCenter.NET
{
    public class ConsoleManager : BaseRunner
    {
        #region Properties

        public ActionsManager ActionsManager { get; set; }

        #endregion

        #region Events

        public event EventHandler<ConsoleEventArgs> NewOutput;
        private void Print(string text) => NewOutput?.Invoke(this, new ConsoleEventArgs(text));

        #endregion

        #region Public methods

        public override string[] GetSupportedCommands() => new[]
        {
            "/add text full-command",
            "/add text",
            "/add",
            "/show",
            "/show text",
            "/edit",
            "/edit text",
            "/edit text full-new-command",
            "/redirect to from1 from2 from3",
        };

        public Command GetCommand(string key)
        {
            var data = ActionsManager.GetCommand(key);
            if (string.IsNullOrWhiteSpace(data))
            {
                return new Command(key);
            }

            var keys = ActionsManager.GetCommands().Where(i => string.Equals(data, i.Item2)).Select(i => i.Item1).ToList();
            return new Command(keys, data);
        }

        public void SaveCommand(Command command)
        {
            // TODO: delete initial command keys
            foreach (var key in command.Keys)
            {
                ActionsManager.SetCommand(key, command.Data);
            }

            if (!command.Keys.Any())
            {
                return;
            }

            Print($"Command \"{command.Keys.First()}\" saved");
        }

        #endregion

        #region Protected methods

        protected void ShowChangeCommandWindow(Command command)
        {
            var window = new ChangeCommandWindow(command);
            if (window.ShowDialog() != true)
            {
                return;
            }

            SaveCommand(window.Command);
        }

        protected override void RunInternal(string text)
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

            (var prefix, var postfix) = text.SplitOnlyFirst(' ');
            var command = prefix.ToLowerInvariant().Substring(1);
            switch (command)
            {
                case "add":
                    AddCommand(postfix);
                    break;

                case "edit":
                    EditCommand(postfix);
                    break;

                case "show":
                    ShowCommand();
                    break;

                case "redirect":
                    RedirectCommand(postfix);
                    break;


                default:
                    Print($"Command is not exists: {command}");
                    break;

            }

        }

        #region Commands

        private void AddCommand(string postfix)
        {
            if (string.IsNullOrWhiteSpace(postfix))
            {
                ShowChangeCommandWindow(new Command());
                return;
            }

            (var key, var arguments) = postfix.SplitOnlyFirst(' ');
            if (string.IsNullOrWhiteSpace(arguments))
            {
                ShowChangeCommandWindow(GetCommand(key));
                return;
            }

            ShowChangeCommandWindow(new Command(key, arguments));
        }

        private void EditCommand(string postfix)
        {
            if (string.IsNullOrWhiteSpace(postfix))
            {
                return; // TODO: Currently is not supported
            }

            (var key, var arguments) = postfix.SplitOnlyFirst(' ');
            if (string.IsNullOrWhiteSpace(arguments))
            {
                ShowChangeCommandWindow(GetCommand(key));
                return;
            }

            //ShowChangeCommandWindow(new Command(key, arguments)); // TODO: Currently is not supported
        }

        private void ShowCommand()
        {
            var text = string.Join(Environment.NewLine,
                ActionsManager.GetCommands().Select(pair => $"{pair.Item1} {pair.Item2}"));

            Print($@"Current commands:
{(string.IsNullOrWhiteSpace(text) ? "You have not added any commands yet" : text)}");
        }

        private void RedirectCommand(string postfix)
        {
            (var name, var arguments) = postfix.SplitOnlyFirst(' ');
            var alternativeNames = arguments.Split(' ');
            foreach (var alternativeName in alternativeNames)
            {
                ActionsManager.SetCommand(alternativeName, ActionsManager.GetCommand(name));
            }
        }


        #endregion

        #endregion
    }
}
