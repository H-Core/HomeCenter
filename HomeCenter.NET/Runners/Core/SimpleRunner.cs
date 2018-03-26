using H.Storages;
using VoiceActions.NET.Utilities;

namespace HomeCenter.NET.Runners.Core
{
    public class SimpleRunner : BaseRunner
    {
        #region Private methods

        protected override void RunInternal(string key, Command command)
        {
            foreach (var line in command.Commands)
            {
                RunSingleLine(line.Text);
            }
        }

        private void RunSingleLine(string command)
        {
            (var prefix, var postfix) = command.SplitOnlyFirstIgnoreQuote(' ');

            var action = GetHandler(prefix).action;
            if (action == null)
            {
                //Log
            }

            action?.Invoke(postfix);
        }

        #endregion
    }
}
