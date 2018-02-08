using System.Collections.Generic;

namespace VoiceActions.NET.Utilities
{
    public class InvariantStringDictionary<T> : Dictionary<string, T>
    {
        #region Public methods

        public new T this[string key] {
            get => base[ToInvariantString(key)];
            set => base[ToInvariantString(key)] = value;
        }

        public new bool ContainsKey(string key) => base.ContainsKey(ToInvariantString(key));

        public new bool TryGetValue(string key, out T value)
        {
            if (!ContainsKey(key))
            {
                value = default(T);
                return false;
            }
            
            value = this[key];
            return true;
        }

        #endregion

        #region Private methods

        private string ToInvariantString(string text) => text.ToLowerInvariant();

        #endregion
    }
}
