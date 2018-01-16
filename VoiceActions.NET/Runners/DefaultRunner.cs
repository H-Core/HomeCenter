using System.Diagnostics;
using VoiceActions.NET.Runners.Core;

namespace VoiceActions.NET.Runners
{
    public class DefaultRunner : BaseRunner
    {
        #region Constructors

        public DefaultRunner()
        {
            Action = RunInternal;
        }

        #endregion

        #region Public methods

        public override string[] GetSupportedCommands() => new[]
        {
            "RUN program.exe arguments"
        };

        #endregion

        #region Private methods

        private static void RunInternal(string command)
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
            (var prefix, var postfix) = command.SplitOnlyFirst(' ');

            Process.Start(prefix, postfix);
        }

        #endregion
    }
}
