using System.Collections.Generic;
using System.Linq;
using VoiceActions.NET.Storages;

namespace VoiceActions.NET.Managers
{
    public class Manager<T> : BaseManager
    {
        #region Properties

        public IStorage<T> Storage { get; set; }

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
            NewText += OnNewText;
        }

        #endregion

        #region Public methods

        public void SetValue(string key, T command) => Storage[key] = command;

        public bool IsHandled(string key) => !string.IsNullOrWhiteSpace(key) && Storage.ContainsKey(key);

        public List<(string, T)> GetValues() =>
            Storage.Select(pair => (pair.Key, pair.Value)).ToList();

        public T GetValue(string key) =>
            Storage.TryGetValue(key, out var result) ? result : default(T);

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
