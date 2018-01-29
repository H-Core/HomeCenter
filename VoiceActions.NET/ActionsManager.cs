using System;
using System.Collections.Generic;
using System.Linq;
using VoiceActions.NET.Utilities;

namespace VoiceActions.NET
{
    public class ActionsManager : VoiceManager
    {
        #region Properties

        private InvariantStringDictionary<Action> ActionDictionary { get; } = new InvariantStringDictionary<Action>();

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

        public void SetAction(string text, Action action) => ActionDictionary[text] = action;

        public bool IsHandled(string text) => ActionDictionary.ContainsKey(text);

        public List<(string, Action)> GetActions() =>
            ActionDictionary.Select(pair => (pair.Key, pair.Value)).ToList();

        public Action GetAction(string text) =>
            ActionDictionary.TryGetValue(text, out var result) ? result : null;

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

            var isHandled = ActionDictionary.ContainsKey(text);
            if (isHandled)
            {
                ActionDictionary[text]?.Invoke();
            }

            OnText(isHandled);
        }

        #endregion
    }
}
