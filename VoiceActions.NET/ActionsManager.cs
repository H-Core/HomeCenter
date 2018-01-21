using System;
using System.Collections.Generic;
using System.Linq;
using VoiceActions.NET.Converters;
using VoiceActions.NET.Recorders;
using VoiceActions.NET.Runners;
using VoiceActions.NET.Runners.Core;

namespace VoiceActions.NET
{
    public class ActionsManager : VoiceManager
    {
        #region Fields

        private Dictionary<string, Action> ActionDictionary { get; } = new Dictionary<string, Action>();
        private IRunner _runner = new DefaultRunner();

        #endregion

        #region Properties

        public IRunner Runner {
            get => _runner;
            set => _runner = value ?? throw new Exception("Runner is null");
        }

        private Dictionary<string, string> CommandsDictionary { get; } = new Dictionary<string, string>();

        #endregion

        #region Events

        public event EventHandler<VoiceActionsEventArgs> NotHandledText;
        public event EventHandler<VoiceActionsEventArgs> HandledText;
        private void OnText(bool isHandled)
        {
            if (isHandled)
            {
                HandledText?.Invoke(this, CreateArgs());
            }
            else
            {
                NotHandledText?.Invoke(this, CreateArgs());
            }
        }

        #endregion

        #region Constructors

        public ActionsManager()
        {
            Initialize();
        }

        public ActionsManager(IRecorder recorder, IConverter converter, IRunner runner) : base(recorder, converter)
        {
            Runner = runner;

            Initialize();
        }

        private void Initialize() => NewText += OnNewText;

        #endregion

        #region Public methods

        public void SetCommand(string text, string command) => CommandsDictionary[ToInvariantString(text)] = command;
        public void SetAction(string text, Action action) => ActionDictionary[ToInvariantString(text)] = action;

        public bool IsHandled(string text)
        {
            var key = ToInvariantString(text);

            return CommandsDictionary.ContainsKey(key) || ActionDictionary.ContainsKey(key);
        }

        public List<(string, string)> GetCommands() =>
            CommandsDictionary.Select(pair => (pair.Key, pair.Value)).ToList();

        public List<(string, Action)> GetActions() =>
            ActionDictionary.Select(pair => (pair.Key, pair.Value)).ToList();

        public string Export() => string.Join(Environment.NewLine + Environment.NewLine,
            CommandsDictionary.Select(pair => $"{pair.Key};{pair.Value}"));

        public void Import(string data)
        {
            var commands = data
                .Split(new[] {Environment.NewLine + Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.SplitOnlyFirst(';'));

            foreach ((var name, var command) in commands)
            {
                SetCommand(name, command);
            }
        }

        #endregion

        #region Event handlers

        private void OnNewText(object sender, VoiceActionsEventArgs args)
        {
            var text = args.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                OnText(false);
                return;
            }

            var key = ToInvariantString(text);
            var isHandled = false;
            if (CommandsDictionary.ContainsKey(key))
            {
                isHandled = true;

                var command = CommandsDictionary[key];
                Runner?.Run(command);
            }
            if (ActionDictionary.ContainsKey(key))
            {
                isHandled = true;

                ActionDictionary[key]?.Invoke();
            }

            OnText(isHandled);
        }

        #endregion

        #region Private methods

        private string ToInvariantString(string text) => text.ToLowerInvariant();

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            _runner?.Dispose();
            _runner = null;

            base.Dispose();
        }

        #endregion
    }
}
