using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace H.NET.Plugins
{
    public class SettingsFile<T> where T : new()
    {
        private string FilePath { get; }
        public Dictionary<string, T> Items { get; set; } = new Dictionary<string, T>();
        private Func<T, string> NameSelector { get; }

        public SettingsFile(string path, Func<T, string> nameSelector)
        {
            FilePath = path ?? throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                throw new ArgumentException("Path is empty", nameof(FilePath));
            }
            NameSelector = nameSelector ?? throw new ArgumentNullException(nameof(nameSelector));
        }

        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                return;
            }

            var text = File.ReadAllText(FilePath);
            var list = JsonConvert.DeserializeObject<List<T>>(text);

            Items = list
                //.Where(i => !string.IsNullOrWhiteSpace(NameSelector(i)))
                .ToDictionary(NameSelector, i => i);
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

        public T GetOrAdd(string name)
        {
            if (!Contains(name))
            {
                Add(name, new T());
            }

            return Get(name);
        }

        public void Add(string name, T item)
        {
            Items[name] = item;

            Save();
        }

        public void Add(T name)
        {
            var key = NameSelector(name);

            Add(key, name);
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