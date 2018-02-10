using System;
using System.Collections.Generic;
using System.Linq;
using HomeCenter.NET.Extensions;
using HomeCenter.NET.Runners.Core;
using HomeCenter.NET.Utilities;
using VoiceActions.NET.Managers;
using VoiceActions.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public class ConsoleRunner : BaseRunner
    {
        #region Properties

        public Manager<Command> Manager { get; set; }

        public List<string> History { get; } = new List<string>();

        #endregion

        #region Events

        public event EventHandler<ConsoleEventArgs> NewOutput;
        private void Print(string text) => NewOutput?.Invoke(this, new ConsoleEventArgs(text));

        #endregion

        #region Public methods

        public override string[] GetSupportedCommands() => new[]
        {
            "/?",
            "/add text full-command",
            "/add text",
            "/add",
            "/show",
            "/show text",
            "/edit",
            "/edit text",
            "/edit text full-new-command",
            "/save",
            "/redirect to from1 from2 from3",
        };

        public Command GetCommand(string key)
        {
            if (!Manager.Storage.ContainsKey(key))
            {
                Manager.Storage[key] = new Command(key);
            }

            return Manager.Storage[key];
        }

        public void SaveCommand(Command command)
        {
            // TODO: delete initial command keys
            foreach (var key in command.Keys)
            {
                Manager.Storage[key] = command;
            }

            if (!command.Keys.Any())
            {
                return;
            }

            Print($"Command \"{command.Keys.First()}\" saved");
            Save();
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

            History.Add(text);

            if (!text.StartsWith("/"))
            {
                Manager.ProcessText(text);
                return;
            }

            (var prefix, var postfix) = text.SplitOnlyFirstIgnoreQuote(' ');
            var command = prefix.ToLowerInvariant().Substring(1);
            switch (command)
            {
                case "?":
                    Print(GetSupportedCommandsText());
                    break;

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

                case "save":
                    Save();
                    Print("Commands saved");
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

            (var key, var arguments) = postfix.SplitOnlyFirstIgnoreQuote(' ');
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

            (var key, var arguments) = postfix.SplitOnlyFirstIgnoreQuote(' ');
            if (string.IsNullOrWhiteSpace(arguments))
            {
                ShowChangeCommandWindow(GetCommand(key));
                //return;
            }

            //ShowChangeCommandWindow(new Command(key, arguments)); // TODO: Currently is not supported
        }

        private void ShowCommand()
        {
            var text = string.Join(Environment.NewLine,
                Manager.Storage.UniqueValues(command => command.Value.Data).Select(command => $"({string.Join(", ", command.Value.Keys)}) {command.Value.Data}"));

            Print($@"Current commands:
{(string.IsNullOrWhiteSpace(text) ? "You have not added any commands yet" : text)}");
        }

        private void Save() => Manager.Storage.Save();

        private void RedirectCommand(string postfix)
        {
            (var name, var arguments) = postfix.SplitOnlyFirstIgnoreQuote(' ');
            var alternativeNames = arguments.Split(' ');
            foreach (var alternativeName in alternativeNames)
            {
                Manager.Storage[alternativeName] = Manager.Storage[name];
            }
        }

        #endregion

        #endregion
    }
}
