using System.Collections.Generic;

namespace H.NET.Core
{
    public interface IStorage<T> : IDictionary<string, T> //IEnumerable<KeyValuePair<string, T>>
    {
        //T this[string key] { get; set; }
        //bool ContainsKey(string key);
        //bool TryGetValue(string key, out T value);
        //bool Remove(string key);

        void Load();
        void Save();
    }
}
