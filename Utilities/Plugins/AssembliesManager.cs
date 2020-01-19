using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using H.NET.Utilities.Plugins.Extensions;
using H.NET.Utilities.Plugins.Utilities;

namespace H.NET.Utilities.Plugins
{
    public class AssembliesManager
    {
        #region Properties

        public static string AssembliesSubFolder { get; } = "Assemblies";
        public static string ActiveAssembliesSubFolderPrefix { get; } = "ActiveAssemblies_";
        public const string AssembliesFileName = "Assemblies.json";
        public const string TempSubFolder = "Temp";

        public string CompanyName { get; }

        public string AppDataFolder { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string BaseFolder { get; }
        public string AssembliesFolder { get; }
        public string ActiveFolder { get; private set; } = string.Empty;
        public string TempFolder { get; }

        public string AssembliesFilePath { get; }
        public SettingsFile<AssemblySettings> AssembliesSettingsFile { get; }
        public Dictionary<string, AssemblySettings> AssembliesSettings => AssembliesSettingsFile.Items;

        public Dictionary<string, Assembly> ActiveAssemblies { get; private set; } = new Dictionary<string, Assembly>();

        private string GetTempDirectory() => DirectoryUtilities.CombineAndCreateDirectory(TempFolder, $"{new Random().Next()}");

        public static Action<string> LogAction { get; set; }
        public static void Log(string text) => LogAction?.Invoke(text);

        #endregion

        #region Constructors

        public AssembliesManager(string companyName)
        {
            CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));

            BaseFolder = DirectoryUtilities.CombineAndCreateDirectory(AppDataFolder, CompanyName);
            AssembliesFolder = DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, AssembliesSubFolder);
            TempFolder = DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, TempSubFolder);

            AssembliesFilePath = Path.Combine(AssembliesFolder, AssembliesFileName);
            AssembliesSettingsFile = new SettingsFile<AssemblySettings>(AssembliesFilePath, i => i.Name);
        }

        #endregion

        #region Public methods

        #region Load/Save

        public virtual void Load()
        {
            TryClean();

            AssembliesSettingsFile.Load();

            ActiveFolder = CreateActiveFolder();
            DirectoryUtilities.Copy(AssembliesFolder, ActiveFolder, true);

            ActiveAssemblies = AssembliesSettings
                .Select(item => Path.Combine(ActiveFolder, item.Value.SubPath))
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Where(File.Exists)
                .Select(LoadAssembly)
                .ToDictionary(i => i.GetSimpleName(), i => i);
        }

        public virtual void Save()
        {
            TryClean();
            
            AssembliesSettingsFile.Save();
        }

        #endregion

        #region Install/Deinstall

        private void AddAssembly(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var settings = AssembliesSettingsFile.GetOrAdd(name);

            settings.Name = name;
            settings.OriginalPath = path;
            settings.ModifiedTime = File.GetLastWriteTimeUtc(path);
            settings.SubPath = Path.Combine(name, Path.GetFileName(path));
        }

        public Assembly Install(string originalPath)
        {
            TryClean();

            AddAssembly(originalPath);
            Save();

            var tempPath = DirectoryUtilities.GetFileCopyFromTempWithOtherFiles(originalPath, GetTempDirectory());
            var assembly = Assembly.LoadFrom(tempPath);

            var toFolder = GetAssemblyFolder(assembly);
            var fromFolder = assembly.GetFolder();
            var paths = assembly.GetDllPaths();

            foreach (var path in paths)
            {
                var directory = Path.GetFileName(Path.GetDirectoryName(path));
                var name = Path.GetFileName(path) ?? throw new Exception("Invalid file name");

                // TODO: only for Emgu.CV.World. Or add special checkbox
                if (string.Equals(directory, "x86", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(directory, "x64", StringComparison.OrdinalIgnoreCase))
                {
                    name = Path.Combine(directory, name);
                }

                var fromPath = File.Exists(path) ? path : Path.Combine(fromFolder, name);
                var toPath = Path.Combine(toFolder, name);

                Directory.CreateDirectory(Path.GetDirectoryName(toPath) ?? "");
                File.Copy(fromPath, toPath, true);
            }

            Load();

            return assembly;
        }

        public void Uninstall(string name)
        {
            TryClean();

            var toFolder = GetAssemblyFolder(name);
            Directory.Delete(toFolder, true);

            AssembliesSettingsFile.Delete(name);

            Load();
        }

        public void Uninstall(Assembly assembly) => Uninstall(assembly.GetSimpleName());
        public void Uninstall(Type type) => Uninstall(type.Assembly);
        public void Uninstall(object obj) => Uninstall(obj.GetType());

        #endregion
        
        #region Update

        public bool UpdatingIsNeed(string name)
        {
            var settings = AssembliesSettingsFile.Get(name);
            var path = settings.OriginalPath;

            if (string.IsNullOrWhiteSpace(path) ||
                !File.Exists(path))
            {
                return false;
            }

            var time = settings.ModifiedTime;
            var checkTime = File.GetLastWriteTimeUtc(path);
            if (time == checkTime)
            {
                return false;
            }

            return true;
        }

        public void Update(string name)
        {
            if (!UpdatingIsNeed(name))
            {
                return;
            }

            var settings = AssembliesSettingsFile.Get(name);
            var path = settings.OriginalPath;

            Uninstall(name);
            Install(path);
        }
        
        #endregion

        protected static object CreateInstance(Type type) => Activator.CreateInstance(type);
        protected static T CreateInstance<T>(Type type) => (T)CreateInstance(type);

        #endregion

        #region Private methods
        
        private string CreateActiveFolder() => DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, $"{ActiveAssembliesSubFolderPrefix}{new Random().Next()}");
        private string GetAssemblyFolder(string name) => DirectoryUtilities.CombineAndCreateDirectory(AssembliesFolder, name);
        private string GetAssemblyFolder(Assembly assembly) => GetAssemblyFolder(assembly.GetSimpleName());

        private static Assembly LoadAssembly(string path) => Assembly.LoadFrom(path);

        protected void TryClean()
        {
            // Clear temp folder files
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

            // Clear temp folders
            Directory.EnumerateDirectories(TempFolder, "*", SearchOption.AllDirectories).AsParallel().ForAll(path =>
            {
                try
                {
                    Directory.Delete(path);
                }
                catch (Exception)
                {
                    //ignored
                }
            });

            // Clear unused active folders
            foreach (var directory in Directory.EnumerateDirectories(BaseFolder, ActiveAssembliesSubFolderPrefix + "*"))
            {
                if (string.Equals(ActiveFolder, directory, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    Directory.Delete(directory, true);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        #endregion
    }
}
