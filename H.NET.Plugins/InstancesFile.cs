using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace H.NET.Plugins
{
    public class InstancesFile
    {
        private string FilePath { get; }
        public Dictionary<string, Instance> Items { get; } = new Dictionary<string, Instance>();

        public InstancesFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is empty", nameof(path));
            }

            FilePath = path;

            if (!File.Exists(path))
            {
                return;
            }

            var text = File.ReadAllText(path);
            var list = JsonConvert.DeserializeObject<List<Instance>>(text);

            Items = list.ToDictionary(i => i.Name, i => i);
        }

        public void Save()
        {
            var list = Items.Select(i => i.Value).ToList();
            var text = JsonConvert.SerializeObject(list, Formatting.Indented);

            File.WriteAllText(FilePath, text);
        }

        public bool Contains(string name) => Items.ContainsKey(name);

        public Instance Get(string name) => Items.TryGetValue(name, out var result)
            ? result : throw new InstanceNotFoundException(name);

        public void Add(string name, string typeName, bool isEnabled = true)
        {
            Items[name] = new Instance
            {
                Name = name,
                TypeName = typeName,
                IsEnabled = isEnabled
            };
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