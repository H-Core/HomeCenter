using System.Diagnostics;
using VoiceActions.NET.Runners.Core;
using VoiceActions.NET.Utilities;

namespace VoiceActions.NET.Runners
{
    public class DefaultRunner : BaseRunner
    {
        #region Public methods

        public override string[] GetSupportedCommands() => new[]
        {
            "RUN program.exe arguments"
        };

        #endregion

        #region Private methods

        protected override void RunInternal(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            (var prefix, var postfix) = command.SplitOnlyFirst(' ');

            switch (prefix.ToLowerInvariant())
            {
                case "run":
                    RunProcess(postfix);
                    break;
            }
        }

        private static void RunProcess(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            (var prefix, var postfix) = command.SplitOnlyFirst(' ');

            Process.Start(prefix, postfix);
        }

        #endregion
    }
}
