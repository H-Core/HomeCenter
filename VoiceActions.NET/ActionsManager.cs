using System;
using System.Collections.Generic;
using System.Linq;
using VoiceActions.NET.Runners;
using VoiceActions.NET.Utilities;

namespace VoiceActions.NET
{
    public class ActionsManager : VoiceManager
    {
        #region Fields

        private IRunner _runner;

        #endregion

        #region Properties

        public IRunner Runner
        {
            get => _runner;
            set {
                if (value is DefaultRunner defaultRunner)
                {
                    defaultRunner.Dictionary = CommandsDictionary;
                }
                _runner = value;
            }
        }

        private InvariantStringDictionary<Action> ActionDictionary { get; } = new InvariantStringDictionary<Action>();
        private InvariantStringDictionary<string> CommandsDictionary { get; } = new InvariantStringDictionary<string>();

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
            Runner = _runner ?? new DefaultRunner();
            NewText += OnNewText;
        }

        #endregion

        #region Public methods

        public void SetCommand(string text, string command) => CommandsDictionary[text] = command;
        public void SetAction(string text, Action action) => ActionDictionary[text] = action;

        public bool IsHandled(string text) => 
            CommandsDictionary.ContainsKey(text) || ActionDictionary.ContainsKey(text);

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

            var isHandled = false;
            if (CommandsDictionary.ContainsKey(text))
            {
                isHandled = true;

                var command = CommandsDictionary[text];
                Runner?.Run(command);
            }
            if (ActionDictionary.ContainsKey(text))
            {
                isHandled = true;

                ActionDictionary[text]?.Invoke();
            }

            OnText(isHandled);
        }

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
