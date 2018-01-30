using System;
using System.Collections.Generic;
using System.Linq;
using HomeCenter.NET.Runners;
using Newtonsoft.Json;
using VoiceActions.NET;
using VoiceActions.NET.Utilities;

namespace HomeCenter.NET
{
    public class RunnerManager : VoiceManager
    {
        #region Properties

        public IRunner Runner { get; set; } = new DefaultRunner();

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

        public RunnerManager()
        {
            NewText += OnNewText;
        }

        #endregion

        #region Public methods

        public void SetCommand(string text, string command) => CommandsDictionary[text] = command;

        public bool IsHandled(string text) => CommandsDictionary.ContainsKey(text);

        public List<(string, string)> GetCommands() =>
            CommandsDictionary.Select(pair => (pair.Key, pair.Value)).ToList();

        public string GetCommand(string text) =>
            CommandsDictionary.TryGetValue(text, out var result) ? result : null;

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

            var isHandled = CommandsDictionary.ContainsKey(text);
            if (isHandled)
            {
                var command = CommandsDictionary[text];
                Runner?.Run(command);
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
