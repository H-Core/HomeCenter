using H.NET.Core.Storages;

namespace H.NET.Core.Managers
{
    public class Manager<T> : BaseManager
    {
        #region Properties

        public IStorage<T> Storage { get; }

        #endregion

        #region Events

        public event TextDelegate NotHandledText;
        public event TextDelegate HandledText;

        public delegate void ValueDelegate(string key, T value);
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
                NotHandledText?.Invoke(text);
                return;
            }

            HandledText?.Invoke(text);

            var value = Storage[text];
            NewValue?.Invoke(text, value);
        }

        #endregion
    }
}
