using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H.NET.Plugins.Extensions;
using H.NET.Plugins.Utilities;
using Newtonsoft.Json;

namespace H.NET.Plugins
{
    public class PluginsManager<T> : AssembliesManager, IDisposable where T : class
    {
        #region Properties

        public const string InstancesSubFolder = "Instances";
        public const string SettingsExtension = ".json";
        public const string InstancesFileName = "Instances.json";

        public Action<T, IEnumerable<SettingItem>> LoadAction { get; }
        public Func<T, IEnumerable<SettingItem>> SaveFunc { get; }

        public string SettingsFolder { get; }
        public string InstancesFilePath { get; }

        private string GetSettingsFilePath(string name) => Path.Combine(SettingsFolder, $"{name}{SettingsExtension}");

        public List<Type> AvailableTypes { get; private set; } = new List<Type>();
        private Type GetTypeByFullName(string name) => AvailableTypes
            .FirstOrDefault(i => string.Equals(i.FullName, name, StringComparison.OrdinalIgnoreCase));

        public Instances<T> Instances { get; private set; }

        #endregion

        #region Constructors

        public PluginsManager(string companyName, Action<T, IEnumerable<SettingItem>> loadAction, Func<T, IEnumerable<SettingItem>> saveFunc) : base(companyName)
        {
            LoadAction = loadAction;
            SaveFunc = saveFunc;

            SettingsFolder = DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, InstancesSubFolder);
            InstancesFilePath = Path.Combine(SettingsFolder, InstancesFileName);
            Instances = new Instances<T>(InstancesFilePath);
        }

        #endregion

        #region Public methods

        #region Load/Save

        public override void Load()
        {
            try
            {
                base.Load();

                //Dispose();

                Instances.Load();
                LoadPlugins();
            }
            catch (Exception exception)
            {
                Log($"Load Plugins: {exception}");
            }

            TryClean();
        }

        public override void Save()
        {
            try
            {
                base.Save();

                foreach (var pair in Instances.Objects)
                {
                    SavePluginSettings(pair.Key, pair.Value.Value);
                }
            }
            catch (Exception exception)
            {
                Log($"Save Plugins: {exception}");
            }

            TryClean();
        }

        #endregion

        public List<KeyValuePair<string, RuntimeObject<T1>>> GetPlugins<T1>() where T1 : class, T => Instances
            .Objects
            .Where(i => i.Value.Exception == null && typeof(T1).IsAssignableFrom(i.Value.Type))
            .Select(i => new KeyValuePair<string, RuntimeObject<T1>>(i.Key, new RuntimeObject<T1>(i.Value.Value as T1, i.Value.Exception)))
            .ToList();

        public List<KeyValuePair<string, RuntimeObject<T1>>> GetEnabledPlugins<T1>() where T1 : class, T =>
            GetPlugins<T1>().Where(i => i.Value.IsEnabled).ToList();

        public RuntimeObject<T1> GetPlugin<T1>(string name) where T1 : class, T =>
            GetPlugins<T1>()
            .FirstOrDefault(i => string.Equals(i.Key, name, StringComparison.OrdinalIgnoreCase))
            .Value;

        private void CreateSettingsFile(string name)
        {
            var path = GetSettingsFilePath(name);
            if (!File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
            }
        }

        private void AddInstance(string name, string typeName, bool isEnabled)
        {
            Instances.Add(name, typeName, isEnabled);
            CreateSettingsFile(name);

            SetInstanceIsEnabled(name, isEnabled);
        }

        public void AddStaticInstance(string name, T obj)
        {
            CreateSettingsFile(name);

            Instances.AddObject(name);
            var instanceObject = Instances.GetObject(name);
            instanceObject.Type = obj.GetType();
            instanceObject.IsStatic = true;

            try
            {
                LoadPluginSettings(name, obj);

                instanceObject.Value = obj;
            }
            catch (Exception exception)
            {
                instanceObject.Exception = exception;
            }
        }

        public void RenameInstance(string name, string newName)
        {
            if (!Instances.Contains(name))
            {
                throw new InstanceNotFoundException(name);
            }
            if (Instances.Contains(newName))
            {
                throw new Exception($"Instance name \"{newName}\" already used");
            }

            var path = GetSettingsFilePath(name);
            var newPath = GetSettingsFilePath(newName);
            if (!File.Exists(newPath))
            {
                File.Copy(path, newPath);
                File.Delete(path);
            }

            var instanceSettings = Instances.GetSettings(name);
            var isEnabledTemp = instanceSettings.IsEnabled;

            SetInstanceIsEnabled(name, false);
            DeleteInstance(name);
            AddInstance(newName, instanceSettings.TypeName, isEnabledTemp);
        }

        public void AddInstance(string name, Type type, bool isEnabled) => AddInstance(name, type.FullName, isEnabled);

        public void AddInstancesFromAssembly(string path, Type interfaceType, Predicate<Type> filter = null)
        {
            var assembly = Install(path);
            var types = assembly.GetTypesOfInterface(interfaceType);
            foreach (var type in types)
            {
                if (TypeIsAvailable(type) &&
                    filter?.Invoke(type) != false)
                {
                    AddInstance(type.Name, type, true);
                }
            }
        }

        public void DeleteInstance(string name)
        {
            if (!Instances.Contains(name))
            {
                throw new InstanceNotFoundException(name);
            }

            SetInstanceIsEnabled(name, false);

            Instances.Delete(name);
        }

        public void SetInstanceIsEnabled(string name, bool value)
        {
            if (!Instances.Contains(name))
            {
                throw new InstanceNotFoundException(name);
            }

            var instanceObject = Instances.GetObject(name);
            var instanceSettings = Instances.GetSettings(name);
            var typeName = instanceSettings.TypeName;
            var type = GetTypeByFullName(typeName);
            instanceObject.Type = type;
            if (value)
            {
                try
                {
                    if (type == null)
                    {
                        //Log($"Load Plugins: Type \"{typeName}\" is not found in current assemblies");

                        instanceObject.Exception = new Exception($"Type \"{typeName}\" is not found in current assemblies");
                        return;
                    }

                    var obj = CreateInstance<T>(type);

                    LoadPluginSettings(name, obj);

                    instanceObject.Value = obj;
                }
                catch (Exception exception)
                {
                    instanceObject.Exception = exception;
                }
            }
            else
            {
                instanceObject.Dispose();
            }

            instanceSettings.IsEnabled = instanceObject.IsEnabled;
            Instances.Save();
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
            var list = JsonConvert.DeserializeObject<List<SettingItem>>(text);
            if (list == null || plugin == null)
            {
                return;
            }

            LoadAction?.Invoke(plugin, list);
        }

        private void SavePluginSettings(string name, T plugin)
        {
            if (plugin == null)
            {
                return;
            }

            var list = SaveFunc?.Invoke(plugin);
            if (list == null)
            {
                return;
            }

            var text = JsonConvert.SerializeObject(list, Formatting.Indented);
            var path = GetSettingsFilePath(name);
            File.WriteAllText(path, text);
        }

        #endregion

        private List<Type> GetAvailableTypes() => ActiveAssemblies
            .SelectMany(i => i.Value.GetTypesOfInterface<T>())
            .Where(TypeIsAvailable)
            .ToList();

        private bool TypeIsAvailable(Type type) => type.GetConstructors().Any(c => c.IsPublic && c.GetParameters().Length == 0);

        private void LoadPlugins()
        {
            AvailableTypes = GetAvailableTypes();
            foreach (var pair in Instances.Settings)
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

                SetInstanceIsEnabled(name, instance.IsEnabled);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Instances?.Dispose();
            Instances = null;
        }

        #endregion
    }
}
