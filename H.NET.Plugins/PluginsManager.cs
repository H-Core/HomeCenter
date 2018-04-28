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
        public const string TempSubFolder = "Temp";
        public const string SettingsExtension = ".txt";
        public const string InstancesFileName = "instances.txt";

        public Action<T, string> LoadAction { get; }
        public Func<T, string> SaveFunc { get; }

        public string SettingsFolder { get; }
        public string TempFolder { get; }
        public string InstancesFilePath { get; }

        private string GetSettingsFilePath(string name) => Path.Combine(SettingsFolder, $"{name}{SettingsExtension}");
        private string GetTempDirectory() => DirectoryUtilities.CombineAndCreateDirectory(TempFolder, $"{new Random().Next()}");

        private string GetFileCopyFromTempWithOtherFiles(string path)
        {
            var filename = Path.GetFileName(path);
            var folder = Path.GetDirectoryName(path);
            var temp = GetTempDirectory();
            DirectoryUtilities.Copy(folder, temp);

            return Path.Combine(temp, filename);
        }

        private void TryClean()
        {
            Directory.EnumerateFiles(TempFolder, "*.*", SearchOption.AllDirectories).AsParallel().ForAll(path =>
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {
                    //ignored
                }
            });
        }

        public List<Type> AvailableTypes { get; private set; } = new List<Type>();
        private Type GetTypeByFullName(string name) => AvailableTypes
            .FirstOrDefault(i => string.Equals(i.FullName, name, StringComparison.OrdinalIgnoreCase));

        public Instances<T> Instances { get; private set; }

        #endregion

        #region Constructors

        public PluginsManager(string companyName, Action<T, string> loadAction, Func<T, string> saveFunc) : base(companyName)
        {
            LoadAction = loadAction;
            SaveFunc = saveFunc;

            SettingsFolder = DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, InstancesSubFolder);
            TempFolder = DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, TempSubFolder);
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

                Dispose();

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

        private void AddInstance(string name, string typeName, bool isEnabled)
        {
            Instances.Add(name, typeName, isEnabled);

            var path = GetSettingsFilePath(name);
            if (!File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
            }

            SetInstanceIsEnabled(name, isEnabled);
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

            var instanceInfo = Instances.GetInfo(name);
            var isEnabledTemp = instanceInfo.IsEnabled;

            SetInstanceIsEnabled(name, false);
            DeleteInstance(name);
            AddInstance(newName, instanceInfo.TypeName, isEnabledTemp);
        }

        public void AddInstance(string name, Type type, bool isEnabled) => AddInstance(name, type.FullName, isEnabled);

        public void AddInstancesFromAssembly(string path, Type interfaceType, Predicate<Type> filter = null)
        {
            path = GetFileCopyFromTempWithOtherFiles(path);

            var assembly = InstallAndGet(path);
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
            var instanceInfo = Instances.GetInfo(name);
            var typeName = instanceInfo.TypeName;
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

            instanceInfo.IsEnabled = instanceObject.IsEnabled;
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

        private List<Type> GetAvailableTypes() => ActiveAssemblies
            .SelectMany(i => i.GetTypesOfInterface<T>())
            .Where(TypeIsAvailable)
            .ToList();

        private bool TypeIsAvailable(Type type) => type.GetConstructors().Any(c => c.IsPublic && c.GetParameters().Length == 0);

        private void LoadPlugins()
        {
            Instances = new Instances<T>(InstancesFilePath);
            AvailableTypes = GetAvailableTypes();
            foreach (var pair in Instances.Informations)
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
