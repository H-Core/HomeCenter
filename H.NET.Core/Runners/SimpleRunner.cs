using H.NET.Core.Utilities;

namespace H.NET.Core.Runners
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
            var values = command.SplitOnlyFirstIgnoreQuote(' ');

            var action = GetHandler(values[0]).Action;
            if (action == null)
            {
                //Log
            }

            action?.Invoke(values[1]);
        }

        #endregion
    }
}
