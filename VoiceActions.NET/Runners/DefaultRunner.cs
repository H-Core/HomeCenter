using System.Diagnostics;
using VoiceActions.NET.Runners.Core;
using VoiceActions.NET.Utilities;

namespace VoiceActions.NET.Runners
{
    public class DefaultRunner : BaseRunner
    {
        #region Properties

        public InvariantStringDictionary<string> Dictionary { get; set; }

        #endregion

        #region Constructors

        public DefaultRunner()
        {
            Action = RunInternal;
        }

        #endregion

        #region Public methods

        public override string[] GetSupportedCommands() => new[]
        {
            "RUN program.exe arguments",
            "CASE text"
        };

        #endregion

        #region Private methods

        private void RunInternal(string command)
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

                case "case":
                    RunInternal(Dictionary?[postfix]);
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
