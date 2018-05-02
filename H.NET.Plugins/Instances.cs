using System;
using System.Collections.Generic;

namespace H.NET.Plugins
{
    public class Instances<T> : IDisposable where T : class
    {
        #region Properties

        private SettingsFile<InstanceSettings> File { get; }
        public Dictionary<string, InstanceSettings> Settings => File.Items;
        public Dictionary<string, RuntimeObject<T>> Objects { get; } = new Dictionary<string, RuntimeObject<T>>();

        #endregion

        #region Constructors

        public Instances(string path)
        {
            File = new SettingsFile<InstanceSettings>(path, i => i.Name);
            foreach (var setting in Settings)
            {
                AddObject(setting.Key);
            }
        }

        #endregion

        #region Public methods

        //public void Load()
        //{
        //    File.Load();
        //}

        public void Save()
        {
            File.Save();
        }

        public bool Contains(string name) => File.Contains(name) && Objects.ContainsKey(name);

        public InstanceSettings GetSettings(string name) => File.Get(name);
        public RuntimeObject<T> GetObject(string name) => Objects.TryGetValue(name, out var result)
            ? result : throw new InstanceNotFoundException(name);

        public void Add(string name, string typeName, bool isEnabled)
        {
            File.Add(new InstanceSettings
            {
                Name = name,
                TypeName = typeName,
                IsEnabled = isEnabled
            });

            AddObject(name);
        }

        public void Delete(string name)
        {
            if (!Contains(name))
            {
                throw new InstanceNotFoundException(name);
            }

            File.Delete(name);

            if (Objects.TryGetValue(name, out var obj))
            {
                obj?.Dispose();
                Objects.Remove(name);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (var pair in Objects)
            {
                pair.Value?.Dispose();
            }

            Objects.Clear();
        }

        #endregion

        #region Private methods

        private void AddObject(string name)
        {
            if (Objects.ContainsKey(name))
            {
                return;
            }

            Objects.Add(name, new RuntimeObject<T>());
        }

        #endregion

    }
}