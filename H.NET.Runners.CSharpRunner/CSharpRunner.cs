using System;
using CSScriptLibrary;
using H.NET.Core.Runners;

namespace H.NET.Runners
{
    public class CSharpRunner : Runner
    {
        #region Constructors

        public CSharpRunner()
        {
            AddAction("csharp", CSharpCommand, "code");
            //AddAction("code-from-file", CodeFromFileCommand, "path");
        }

        #endregion

        #region Private methods

        private void CSharpCommand(string text)
        {
            var action = CSScript.LoadDelegate<Action<Action<string>>>($@"
using System;
using System.IO;

void Action(Action<string> Say)
{{
{text}
}}");

            action(Say);
        }

        #endregion
    }
}
