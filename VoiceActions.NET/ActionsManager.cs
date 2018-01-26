using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VoiceActions.NET.Runners;
using VoiceActions.NET.Utilities;

namespace VoiceActions.NET
{
    public class ActionsManager : VoiceManager
    {
        #region Properties

        public IRunner Runner { get; set; } = new DefaultRunner();

        private InvariantStringDictionary<Action> ActionDictionary { get; } = new InvariantStringDictionary<Action>();
        private InvariantStringDictionary<string> CommandsDictionary { get; set; } = new InvariantStringDictionary<string>();

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

        public string GetCommand(string text) =>
            CommandsDictionary.TryGetValue(text, out var result) ? result : null;

        public List<(string, Action)> GetActions() =>
            ActionDictionary.Select(pair => (pair.Key, pair.Value)).ToList();

        public Action GetAction(string text) =>
            ActionDictionary.TryGetValue(text, out var result) ? result : null;

        public string Export() => JsonConvert.SerializeObject(CommandsDictionary);

        public void Import(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                CommandsDictionary.Clear();
                return;
            }

            CommandsDictionary = JsonConvert.DeserializeObject<InvariantStringDictionary<string>>(data);
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
            Runner?.Dispose();
            Runner = null;

            base.Dispose();
        }

        #endregion
    }
}
