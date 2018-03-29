using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace H.NET.Plugins
{
    public class InstancesFile<T> where T : class
    {
        public string FilePath { get; set; }
        public Dictionary<string, Instance<T>> Items { get; }

        public InstancesFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is empty", nameof(path));
            }

            FilePath = path;

            if (File.Exists(path))
            {
                Items = File.ReadAllLines(path).Select(Instance<T>.FromString).ToDictionary(i => i.Name.ToLowerInvariant(), i => i);
            }
        }

        public void Save()
        {
            File.WriteAllLines(FilePath, Items.Select(i => i.Value.ToString()));
        }

        public bool Contains(string name) => Items.ContainsKey(name.ToLowerInvariant());

        public Instance<T> Get(string name) => Items.TryGetValue(name.ToLowerInvariant(), out var result)
            ? result : throw new InstanceNotFoundException(name);

        public void Add(string name, string typeName, bool isEnabled = true)
        {
            Items[name.ToLowerInvariant()] = new Instance<T>
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

        public class InstanceNotFoundException : KeyNotFoundException
        {
            public InstanceNotFoundException(string name) : base($"Instance {name} is not found in the instances file")
            {
            }
        }
    }
}