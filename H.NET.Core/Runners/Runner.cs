using System;
using System.Linq;
using H.NET.Core.Utilities;

namespace H.NET.Core.Runners
{
    public abstract class Runner : Module, IRunner
    {
        #region Properties
        
        private InvariantStringDictionary<RunInformation> HandlerDictionary { get; } = new InvariantStringDictionary<RunInformation>();

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

        public string[] GetSupportedCommands() => HandlerDictionary.Select(i => $"{i.Key} {i.Value.Description}").ToArray();

        public virtual bool IsSupport(string key, string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return false;
            }

            var values = data.SplitOnlyFirst(' ');

            return HandlerDictionary.ContainsKey(values[0]);
        }

        #endregion

        #region Protected methods

        protected virtual RunInformation RunInternal(string key, string data)
        {
            var values = data.SplitOnlyFirstIgnoreQuote(' ');

            var information = GetHandler(values[0]);
            var action = information.Action;
            if (action == null)
            {
                //Log
            }

            try
            {
                action?.Invoke(values[1]);

                return information;
            }
            catch (Exception exception)
            {
                return information.WithException(exception);
            }
        }

        protected void AddAction(string key, Action<string> action, string description = null) =>
            AddAction(key, new RunInformation
            {
                Description = description,
                Action = action,
                IsInternal = false
            });

        protected void AddInternalAction(string key, Action<string> action, string description = null) =>
            AddAction(key, new RunInformation
            {
                Description = description,
                Action = action,
                IsInternal = true
            });

        protected void AddAction(string key, RunInformation information) =>
            HandlerDictionary[key] = information;

        protected RunInformation GetHandler(string key) => 
            HandlerDictionary.TryGetValue(key, out var handler) ? handler : new RunInformation();

        #endregion
    }
}