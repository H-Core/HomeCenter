using System;
using System.Linq;
using System.Windows.Controls;
using VoiceActions.NET;
using VoiceActions.NET.Runners.Core;
using VoiceActions.NET.Utilities;

namespace HomeCenter.NET
{
    public class ConsoleManager : BaseRunner
    {
        #region Properties

        public ActionsManager ActionsManager { get; set; }
        public TextBox ConsoleTextBox { get; set; }

        public InvariantStringDictionary<string> Redirections { get; } = new InvariantStringDictionary<string>();

        #endregion

        #region Public methods

        public void Print(string text) => ConsoleTextBox.Text += $"{DateTime.Now:T}: {text}{Environment.NewLine}";

        public override string[] GetSupportedCommands() => new[]
        {
            "/add text full-command",
            "/show",
            "/redirect to from1 from2 from3",
        };

        #endregion

        #region Protected methods

        protected override void RunInternal(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (Redirections.ContainsKey(text))
            {
                var value = Redirections[text];
                Print($"Redirected from {text} to {value}");
                RunInternal(value);
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
            (var name, var arguments) = postfix.SplitOnlyFirst(' ');
            ActionsManager.SetCommand(name, arguments);
            Print($"Command \"{arguments}\" added");
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
