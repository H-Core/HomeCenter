using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H.NET.Plugins.Extensions;
using H.NET.Plugins.Utilities;

namespace H.NET.Plugins
{
    public class PluginsManager<T> : AssembliesManager, IDisposable where T : class
    {
        #region Properties

        public const string InstancesSubFolder = "Instances";
        public const string SettingsExtension = ".txt";
        public const string InstancesFileName = "instances.txt";

        public Action<T, string> LoadAction { get; }
        public Func<T, string> SaveFunc { get; }

        public string SettingsFolder { get; }
        public string InstancesFilePath { get; }
        public InstancesFile<T> InstancesFile { get; }

        private string GetSettingsFilePath(string name) => Path.Combine(SettingsFolder, $"{name}{SettingsExtension}");

        public List<Type> AvailableTypes { get; private set; } = new List<Type>();
        public Dictionary<string, Instance<T>> Instances { get; private set; } = new Dictionary<string, Instance<T>>();

        #endregion

        #region Constructors

        public PluginsManager(string companyName, Action<T, string> loadAction, Func<T, string> saveFunc) : base(companyName)
        {
            LoadAction = loadAction;
            SaveFunc = saveFunc;

            SettingsFolder = DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, InstancesSubFolder);
            InstancesFilePath = Path.Combine(SettingsFolder, InstancesFileName);
            InstancesFile = new InstancesFile<T>(InstancesFilePath);
        }

        #endregion

        #region Public methods

        #region Load/Save

        public override void Load()
        {
            try
            {
                base.Load();

                Dispose();

                AvailableTypes = ActiveAssemblies.SelectMany(i => i.GetTypesOfInterface<T>()).ToList();
                Instances = LoadPlugins();
            }
            catch (Exception exception)
            {
                Log($"Load Plugins: {exception}");
            }
        }

        public override void Save()
        {
            try
            {
                base.Save();

                foreach (var pair in Instances ?? new Dictionary<string, Instance<T>>())
                {
                    SavePluginSettings(pair.Key, pair.Value.Value);
                }
            }
            catch (Exception exception)
            {
                Log($"Save Plugins: {exception}");
            }
        }

        #endregion

        public List<KeyValuePair<string, T1>> GetPluginsOfSubtype<T1>() where T1 : T
        {
            return Instances
                .Where(i => i.Value.Value is T1)
                .Select(i => new KeyValuePair<string, T1>(i.Key, (T1)i.Value.Value))
                .ToList();
        }

        private void AddInstance(string name, string typeName)
        {
            InstancesFile.Add(name, typeName);

            var path = GetSettingsFilePath(name);
            if (!File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
            }
        }

        public void AddInstance(string name, Type type) => AddInstance(name, type.FullName);

        public void DeleteInstance(string name) => InstancesFile.Delete(name);

        public string GetTypeNameOfName(string name) => InstancesFile.Get(name).TypeName;

        public Type GetTypeOfName(string name) => GetTypeByFullName(GetTypeNameOfName(name));

        public void SetInstanceIsEnabled(string name, bool value)
        {
            name = name.ToLowerInvariant();
            if (!InstancesFile.Contains(name))
            {
                throw new InstancesFile<T>.InstanceNotFoundException(name);
            }

            var instance = InstancesFile.Get(name);
            if (value)
            {
                try
                {
                    var type = GetTypeOfName(name);
                    if (type == null)
                    {
                        //Log($"Load Plugins: Type \"{typeName}\" is not found in current assemblies");

                        instance.SetException(new Exception($"Type \"{GetTypeNameOfName(name)}\" is not found in current assemblies"));
                        return;
                    }

                    var obj = (T)Activator.CreateInstance(type);

                    LoadPluginSettings(name, obj);

                    instance.SetValue(obj);
                }
                catch (Exception exception)
                {
                    instance.SetException(exception);
                }
            }
            else
            {
                instance.Dispose();
            }

            InstancesFile.Save();
        }

        #endregion

        #region Private methods

        #region Load/Save Settings


        private void LoadPluginSettings(string name, T plugin)
        {
            var path = GetSettingsFilePath(name);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return;
            }

            var text = File.ReadAllText(path);
            LoadAction?.Invoke(plugin, text);
        }

        private void SavePluginSettings(string name, T plugin)
        {
            var text = SaveFunc?.Invoke(plugin);
            if (text == null)
            {
                return;
            }

            File.WriteAllText(GetSettingsFilePath(name), text);
        }

        #endregion

        private Type GetTypeByFullName(string name) => AvailableTypes
            .FirstOrDefault(i => string.Equals(i.FullName, name, StringComparison.OrdinalIgnoreCase));

        private Dictionary<string, Instance<T>> LoadPlugins()
        {
            foreach (var pair in InstancesFile.Items)
            {
                var instance = pair.Value;
                var name = instance.Name;
                var typeName = instance.TypeName;
                if (string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(typeName))
                {
                    //Log($"Load Plugins: Invalid file name: {fileName}");

                    continue;
                }

                var type = GetTypeByFullName(typeName);
                if (type == null)
                {
                    //Log($"Load Plugins: Type \"{typeName}\" is not found in current assemblies");
                    instance.SetException(new Exception($"Type \"{typeName}\" is not found in current assemblies"));
                    continue;
                }

                try
                {
                    if (!instance.IsEnabled)
                    {
                        instance.Dispose();
                        continue;
                    }

                    var obj = (T)Activator.CreateInstance(type);

                    LoadPluginSettings(name, obj);

                    instance.SetValue(obj);
                }
                catch (Exception exception)
                {
                    //Log($"Load Plugins: {exception}");

                    instance.SetException(exception);
                }
            }

            return InstancesFile.Items;
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            if (Instances == null)
            {
                return;
            }

            foreach (var plugin in Instances
                .Where(i => i.Value.Value is IDisposable)
                .Select(i => i.Value.Value)
                .Cast<IDisposable>())
            {
                plugin.Dispose();
            }

            Instances.Clear();
        }

        #endregion
    }
}
