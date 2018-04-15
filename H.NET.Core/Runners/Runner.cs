using System;
using System.Linq;
using System.Threading.Tasks;
using H.NET.Core.Utilities;

namespace H.NET.Core.Runners
{
    public abstract class Runner : Module, IRunner
    {
        #region Properties

        private InvariantStringDictionary<RunInformation> HandlerDictionary { get; } = new InvariantStringDictionary<RunInformation>();
        private InvariantStringDictionary<Func<object>> Variables { get; } = new InvariantStringDictionary<Func<object>>();

        #endregion

        #region Events

        public event EventHandler<RunnerEventArgs> BeforeRun;
        private void OnBeforeRun(string text) => BeforeRun?.Invoke(this, CreateArgs(text));

        public event EventHandler<RunnerEventArgs> AfterRun;
        private void OnAfterRun(string text) => AfterRun?.Invoke(this, CreateArgs(text));

        private RunnerEventArgs CreateArgs(string text) => new RunnerEventArgs { Runner = this, Text = text };

        #endregion

        #region Public methods

        public RunInformation Run(string key, string data)
        {
            try
            {
                OnBeforeRun(key);

                var info = RunInternal(key, data);

                OnAfterRun(key);

                return info;
            }
            catch (Exception exception)
            {
                Print($"Exception while running command: \"{data}\": {exception}");

                return new RunInformation(exception);
            }
        }

        public string[] GetSupportedCommands() =>
            HandlerDictionary.Select(i => $"{i.Key} {i.Value.Description}").ToArray();

        public string[] GetVariables() =>
            Variables.Select(i => $"{i.Key} {i.Value?.Invoke()}").ToArray();

        public virtual bool IsSupport(string key, string data) => GetInformation(key, data) != null;
        public bool IsInternal(string key, string data) => GetInformation(key, data)?.IsInternal ?? false;

        public RunInformation GetInformation(string key, string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            var values = data.SplitOnlyFirst(' ');
            return HandlerDictionary.TryGetValue(values[0], out var information) ? information : null;
        }

        #endregion

        #region Protected methods

        private string FindVariablesAndReplace(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return command;
            }

            foreach (var variable in Variables)
            {
                var variableName = variable.Key;
                if (string.IsNullOrWhiteSpace(variableName) || !command.Contains(variableName))
                {
                    continue;
                }

                var func = variable.Value;
                var value = func?.Invoke();

                command = command.Replace(variable.Key, value?.ToString() ?? string.Empty);
            }

            return command;
        }

        protected virtual RunInformation RunInternal(string key, string data)
        {
            var values = data.SplitOnlyFirstIgnoreQuote(' ');
            var information = GetHandler(values[0]);
            var command = values[1];

            try
            {
                command = FindVariablesAndReplace(command);
            }
            catch (Exception exception)
            {
                information.Exception = exception;
            }

            var action = information.Action;
            if (action == null)
            {
                //Log
            }

            try
            {
                action?.Invoke(command);

                information.RunText = $"{values[0]} {command}";

                return information;
            }
            catch (Exception exception)
            {
                return information.WithException(exception);
            }
        }

        protected void AddVariable(string key, Func<string> action) =>
            Variables[key] = action;

        protected void AddAction(string key, Action<string> action, string description = null, bool isInternal = false) =>
            AddAction(key, new RunInformation
            {
                Description = description,
                Action = action,
                IsInternal = isInternal
            });

        protected void AddInternalAction(string key, Action<string> action, string description = null) =>
            AddAction(key, action, description, true);

        protected void AddAsyncAction(string key, Func<string, Task> func, string description = null, bool isInternal = false) =>
            AddAction(key, text => AsyncHelpers.RunSync(() => func?.Invoke(text)), description, isInternal);

        protected void AddInternalAsyncAction(string key, Func<string, Task> func, string description = null) =>
            AddAsyncAction(key, func, description, true);

        protected void AddAction(string key, RunInformation information) =>
            HandlerDictionary[key] = information;

        protected RunInformation GetHandler(string key) =>
            HandlerDictionary.TryGetValue(key, out var handler) ? handler : new RunInformation();

        #endregion
    }
}