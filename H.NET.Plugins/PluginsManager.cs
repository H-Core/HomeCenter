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
        public const string DeletedSubFolder = "Deleted";
        public const string SettingsExtension = ".json";

        public Action<T, string> LoadAction { get; }
        public Func<T, string> SaveFunc { get; }

        public string SettingsFolder { get; }
        public string DeletedSettingsFolder { get; }
        public List<Type> AvailableTypes { get; private set; } = new List<Type>();
        public Dictionary<string, T> ActivePlugins { get; private set; } = new Dictionary<string, T>();

        #endregion

        #region Constructors

        public PluginsManager(string companyName, Action<T, string> loadAction, Func<T, string> saveFunc) : base(companyName)
        {
            LoadAction = loadAction;
            SaveFunc = saveFunc;

            SettingsFolder = DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, InstancesSubFolder);
            DeletedSettingsFolder = DirectoryUtilities.CombineAndCreateDirectory(SettingsFolder, DeletedSubFolder);
        }

        #endregion

        #region Public methods

        #region Load/Save

        public override void Load()
        {
            base.Load();

            Dispose();

            AvailableTypes = ActiveAssemblies.SelectMany(i => i.GetTypesOfInterface<T>()).ToList();
            ActivePlugins = LoadPlugins();
        }

        public override void Save()
        {
            base.Save();

            foreach (var pair in ActivePlugins ?? new Dictionary<string, T>())
            {
                SavePluginSettings(pair.Key, pair.Value);
            }
        }

        #endregion

        public List<KeyValuePair<string, T1>> GetPluginsOfSubtype<T1>() where T1 : T
        {
            return ActivePlugins
                .Where(i => i.Value is T1)
                .Select(i => new KeyValuePair<string, T1>(i.Key, (T1)i.Value))
                .ToList();
        }

        public void AddInstance(string name, string typeName)
        {
            File.WriteAllText(Path.Combine(SettingsFolder, $"{name}-{typeName}{SettingsExtension}"), string.Empty);
        }

        public void AddInstance(string name, Type type) => AddInstance(name, type.FullName);

        public void DeleteInstance(string name) => Directory
            .EnumerateFiles(SettingsFolder, $"*{SettingsExtension}")
            .Where(i => Path.GetFileName(i)?.StartsWith(name) ?? false)
            .AsParallel()
            .ForAll(from =>
            {
                var fileName = Path.GetFileNameWithoutExtension(from);
                var extension = Path.GetExtension(from);
                var to = Path.Combine(DeletedSettingsFolder, $"{fileName}_{new Random().Next()}{extension}");

                File.Copy(from, to, true);
                File.Delete(from);
            });

        #endregion

        #region Private methods

        #region Load/Save Settings

        private string GetDefaultSettingsPath(string name, T plugin) => Path.Combine(SettingsFolder,
            $"{name}-{plugin.GetType().FullName}{SettingsExtension}");

        private void LoadPluginSettings(string name, T plugin, string path = null)
        {
            path = path ?? GetDefaultSettingsPath(name, plugin);

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return;
            }

            var text = File.ReadAllText(path);
            LoadAction?.Invoke(plugin, text);
        }

        private void SavePluginSettings(string name, T plugin, string path = null)
        {
            var text = SaveFunc?.Invoke(plugin);
            if (text == null)
            {
                return;
            }

            path = path ?? GetDefaultSettingsPath(name, plugin);
            File.WriteAllText(path, text);
        }

        #endregion

        private Type GetTypeByFullName(string name) => AvailableTypes
            .FirstOrDefault(i => string.Equals(i.FullName, name, StringComparison.OrdinalIgnoreCase));

        private Dictionary<string, T> LoadPlugins()
        {
            var plugins = new Dictionary<string, T>();
            foreach (var path in Directory.EnumerateFiles(SettingsFolder, $"*{SettingsExtension}"))
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                var values = fileName.Split('-');
                var name = values.ElementAtOrDefault(0);
                var typeName = values.ElementAtOrDefault(1);
                if (string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(typeName))
                {
                    Log($"Load Plugins: Invalids file name: {fileName}");

                    continue;
                }

                var type = GetTypeByFullName(typeName);
                if (type == null)
                {
                    Log($"Load Plugins: Type \"{typeName}\" is not found in current assemblies");

                    plugins.Add(name, null);
                    continue;
                }

                try
                {
                    var obj = (T) Activator.CreateInstance(type);

                    LoadPluginSettings(name, obj);

                    plugins.Add(name, obj);
                }
                catch (Exception exception)
                {
                    Log($"Load Plugins: {exception}");

                    plugins.Add(name, null);
                }
            }

            return plugins;
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            if (ActivePlugins == null)
            {
                return;
            }

            foreach (var plugin in ActivePlugins
                .Where(i => i.Value is IDisposable)
                .Select(i => i.Value)
                .Cast<IDisposable>())
            {
                plugin.Dispose();
            }

            ActivePlugins.Clear();
        }

        #endregion
    }
}
