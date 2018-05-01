using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace H.NET.Plugins
{
    public class ItemsFile<T>
    {
        private string FilePath { get; }
        public Dictionary<string, T> Items { get; private set; } = new Dictionary<string, T>();
        private Func<T, string> KeySelector { get; }

        public ItemsFile(string path, Func<T, string> keySelector)
        {
            FilePath = path ?? throw new ArgumentNullException(nameof(path));
            KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

            Load(path);
        }

        private void Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is empty", nameof(path));
            }

            if (!File.Exists(path))
            {
                return;
            }

            var text = File.ReadAllText(path);
            var list = JsonConvert.DeserializeObject<List<T>>(text);

            Items = list.ToDictionary(KeySelector, i => i);
        }

        public void Save()
        {
            var list = Items.Select(i => i.Value).ToList();
            var text = JsonConvert.SerializeObject(list, Formatting.Indented);

            File.WriteAllText(FilePath, text);
        }

        public bool Contains(string name) => Items.ContainsKey(name);

        public T Get(string name) => Items.TryGetValue(name, out var result)
            ? result : throw new InstanceNotFoundException(name);

        public void Add(T item)
        {
            var key = KeySelector(item);
            Items[key] = item;

            Save();
        }

        public void Delete(string name)
        {
            if (!Contains(name))
            {
                throw new InstanceNotFoundException(name);
            }

            Items.Remove(name);
            Save();
        }

        private class InstanceNotFoundException : KeyNotFoundException
        {
            public InstanceNotFoundException(string name) : base($"Instance {name} is not found in the instances file")
            {
            }
        }
    }
}