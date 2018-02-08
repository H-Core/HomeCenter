using System;
using System.Diagnostics;
using VoiceActions.NET.Runners.Core;
using VoiceActions.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public class DefaultRunner : BaseRunner
    {
        #region Public methods

        public override string[] GetSupportedCommands() => new[]
        {
            "RUN program.exe arguments",
            "SAY text"
        };

        #endregion

        #region Private methods

        protected override void RunInternal(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var commands = text.Split(Environment.NewLine.ToCharArray());
            foreach (var command in commands)
            {
                RunSingleCommand(command);
            }
        }

        private void RunSingleCommand(string command)
        {
            (var prefix, var postfix) = command.SplitOnlyFirstIgnoreQuote(' ');

            switch (prefix.ToLowerInvariant())
            {
                case "run":
                    RunProcess(postfix);
                    break;

                case "say":
                    Say(postfix);
                    break;
            }
        }

        private static void RunProcess(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            (var prefix, var postfix) = command.SplitOnlyFirstIgnoreQuote(' ');

            var path = prefix.Trim('\"', '\\').Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("\\", "/");
            Process.Start(path, postfix);
        }

        #endregion
    }
}
