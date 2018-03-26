using System.Collections.Generic;

namespace H.NET.Core.Utilities
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

        public new bool Remove(string key) => base.Remove(ToInvariantString(key));

        #endregion

        #region Private methods

        private static string ToInvariantString(string text) => text?.ToLowerInvariant();

        #endregion
    }
}
