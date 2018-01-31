using System;
using VoiceActions.NET.Utilities;

namespace VoiceActions.NET
{
    public class ActionsManager : VoiceManager
    {
        #region Properties

        public Action<string> GlobalAction { get; set; }
        public InvariantStringDictionary<Action> Actions { get; } = new InvariantStringDictionary<Action>();

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

        public bool IsHandled(string text) => GlobalAction != null || Actions.ContainsKey(text);

        #endregion

        #region Event handlers

        private void OnNewText(object sender, VoiceActionsEventArgs args)
        {
            var text = args.Text;
            GlobalAction?.Invoke(text);
            if (string.IsNullOrWhiteSpace(text))
            {
                OnText(false);
                return;
            }

            if (Actions.ContainsKey(text))
            {
                Actions[text]?.Invoke();
            }

            OnText(IsHandled(text));
        }

        #endregion
    }
}
