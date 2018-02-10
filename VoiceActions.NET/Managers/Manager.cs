using VoiceActions.NET.Storages;

namespace VoiceActions.NET.Managers
{
    public class Manager<T> : BaseManager
    {
        #region Properties

        public IStorage<T> Storage { get; }

        #endregion

        #region Events

        public event TextDelegate NotHandledText;
        public event TextDelegate HandledText;

        public delegate void ValueDelegate(T value);
        public event ValueDelegate NewValue;

        #endregion

        #region Constructors

        public Manager(IStorage<T> storage = null)
        {
            Storage = storage ?? new InvariantDictionaryStorage<T>();
            Storage.Load();

            NewText += OnNewText;
        }

        #endregion

        #region Event handlers

        private void OnNewText(string text)
        {
            if (string.IsNullOrWhiteSpace(text) ||
                !Storage.ContainsKey(text))
            {
                NotHandledText?.Invoke(Text);
                return;
            }

            HandledText?.Invoke(Text);

            var value = Storage[text];
            NewValue?.Invoke(value);
        }

        #endregion
    }
}
