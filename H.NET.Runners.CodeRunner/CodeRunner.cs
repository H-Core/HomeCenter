using System;
using CSScriptLibrary;
using H.NET.Core.Runners;

namespace H.NET.Runners.CodeRunner
{
    public class CodeRunner : Runner
    {
        #region Constructors

        public CodeRunner()
        {
            AddAction("code", CodeCommand, "code");
            //AddAction("code-from-file", CodeFromFileCommand, "path");
        }

        #endregion

        #region Private methods

        private void CodeCommand(string text)
        {
            var action = CSScript.LoadDelegate<Action>($@"
void Action()
{{
{text}
}}");

            action();
        }

        #endregion
    }
}
