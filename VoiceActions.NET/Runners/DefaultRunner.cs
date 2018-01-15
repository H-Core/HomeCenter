using System.Diagnostics;
using System.Linq;
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

        private void RunInternal(string command)
        {
            (var prefix, var postfix) = GetPrefixPostfix(command, ' ');

            switch (prefix.ToLowerInvariant())
            {
                case "run":
                    RunProcess(postfix);
                    break;
            }
        }

        private void RunProcess(string command)
        {
            (var prefix, var postfix) = GetPrefixPostfix(command, ' ');

            Process.Start(prefix, postfix);
        }

        private (string, string) GetPrefixPostfix(string command, char separator)
        {
            var prefix = command.Split(separator).FirstOrDefault();
            var postfix = command.Replace(prefix, string.Empty).Trim();

            return (prefix, postfix);
        }

        #endregion
    }
}
