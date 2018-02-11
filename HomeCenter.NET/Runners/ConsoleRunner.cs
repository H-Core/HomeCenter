using System;
using System.Linq;
using HomeCenter.NET.Extensions;
using HomeCenter.NET.Runners.Core;
using HomeCenter.NET.Utilities;
using HomeCenter.NET.Windows;
using VoiceActions.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public class ConsoleRunner : BaseRunner
    {
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
            "$key_enter",
            "$key_up"
        };

        public override bool IsSupport(string key, string data) => base.IsSupport(key, key);

        public void SaveCommand(Command command)
        {
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
            if (!ChangeCommandWindow.ShowAndSaveIfNeeded(command, Storage))
            {
                return;
            }

            SaveCommand(command);
        }

        protected override void RunInternal(string key, Command command)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (!key.StartsWith("/"))
            {
                return;
            }

            (var prefix, var postfix) = key.SplitOnlyFirstIgnoreQuote(' ');
            var value = prefix.ToLowerInvariant().Substring(1);
            switch (value)
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
                ShowChangeCommandWindow(new Command(key));
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
                ShowChangeCommandWindow(new Command(key));
                //return;
            }

            //ShowChangeCommandWindow(new Command(key, arguments)); // TODO: Currently is not supported
        }

        private void ShowCommand()
        {
            var text = string.Join(Environment.NewLine,
                Storage.UniqueValues(command => command.Value.Data).Select(command => $"({string.Join(", ", command.Value.Keys)}) {command.Value.Data}"));

            Print($@"Current commands:
{(string.IsNullOrWhiteSpace(text) ? "You have not added any commands yet" : text)}");
        }

        private void Save() => Storage.Save();

        private void RedirectCommand(string postfix)
        {
            (var name, var arguments) = postfix.SplitOnlyFirstIgnoreQuote(' ');
            var alternativeNames = arguments.Split(' ');
            foreach (var alternativeName in alternativeNames)
            {
                Storage[alternativeName] = Storage[name];
            }
        }

        #endregion

        #endregion
    }
}
