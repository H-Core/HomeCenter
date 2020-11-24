using System;
using H.NET.Core.Storages;

namespace H.NET.Core.Managers
{
    public class Manager<T> : BaseManager
    {
        #region Properties

        public IStorage<T> Storage { get; }

        #endregion

        #region Events

        public event EventHandler<string> NotHandledText;
        public event EventHandler<string> HandledText;

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

        private void OnNewText(object sender, string text)
        {
            if (string.IsNullOrWhiteSpace(text) ||
                !Storage.ContainsKey(text))
            {
                NotHandledText?.Invoke(this, text);
                return;
            }

            HandledText?.Invoke(this, text);

            var value = Storage[text];
            NewValue?.Invoke(text, value);
        }

        #endregion
    }
}
