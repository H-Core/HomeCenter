using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using H.NET.Plugins.Extensions;
using H.NET.Plugins.Utilities;

namespace H.NET.Plugins
{
    public class AssembliesManager
    {
        #region Properties

        public static string AssembliesSubFolder { get; } = "Assemblies";
        public static string ActiveAssembliesSubFolderPrefix { get; } = "ActiveAssemblies_";

        public string CompanyName { get; }

        public string AppDataFolder { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string BaseFolder { get; }
        public string AssembliesFolder { get; }

        public string ActiveFolder { get; private set; } = string.Empty;
        public List<Assembly> ActiveAssemblies { get; private set; } = new List<Assembly>();

        public static Action<string> LogAction { get; set; }
        public static void Log(string text) => LogAction?.Invoke(text);

        #endregion

        #region Constructors

        public AssembliesManager(string companyName)
        {
            CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));

            BaseFolder = DirectoryUtilities.CombineAndCreateDirectory(AppDataFolder, CompanyName);
            AssembliesFolder = DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, AssembliesSubFolder);
        }

        #endregion

        #region Public methods

        #region Load/Save

        public virtual void Load()
        {
            TryClean();

            ActiveFolder = CreateActiveFolder();
            DirectoryUtilities.Copy(AssembliesFolder, ActiveFolder, true);

            ActiveAssemblies = Directory
                .EnumerateDirectories(ActiveFolder)
                .Select(folder => Path.Combine(folder, $"{Path.GetFileName(folder) ?? ""}.dll"))
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Where(File.Exists)
                .Select(LoadAssembly)
                .ToList();
        }

        public virtual void Save()
        {
            TryClean();
        }

        #endregion

        #region Install/Deinstall

        public void Install(Assembly assembly)
        {
            TryClean();

            var toFolder = GetAssemblyFolder(assembly);
            var fromFolder = assembly.GetFolder();
            var paths = assembly.GetDllPaths();

            foreach (var path in paths)
            {
                var directory = Path.GetFileName(Path.GetDirectoryName(path));
                var name = Path.GetFileName(path) ?? throw new Exception("Invalid file name");
                if (string.Equals(directory, "x86", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(directory, "x64", StringComparison.OrdinalIgnoreCase))
                {
                    name = Path.Combine(directory, name);
                }

                var fromPath = Path.Combine(fromFolder, name);
                var toPath = Path.Combine(toFolder, name);

                Directory.CreateDirectory(Path.GetDirectoryName(toPath) ?? "");
                File.Copy(fromPath, toPath, true);
            }

            Load();
        }

        public void Install(string path) => Install(Assembly.LoadFile(path));

        public Assembly InstallAndGet(string path)
        {
            var assembly = Assembly.LoadFile(path);

            Install(assembly);

            return assembly;
        } 

        public void Deinstall(Assembly assembly)
        {
            TryClean();

            var toFolder = GetAssemblyFolder(assembly);
            Directory.Delete(toFolder, true);

            Load();
        }

        public void Deinstall(Type type) => Deinstall(type.Assembly);
        public void Deinstall(object obj) => Deinstall(obj.GetType());

        #endregion

        #endregion

        #region Private methods

        private string CreateActiveFolder() => DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, $"{ActiveAssembliesSubFolderPrefix}{new Random().Next()}");
        private string GetAssemblyFolder(Assembly assembly) => DirectoryUtilities.CombineAndCreateDirectory(AssembliesFolder, assembly.GetName().Name);

        /*
        public class ProxyDomain : MarshalByRefObject
        {
            public Assembly GetAssembly(string path)
            {
                try
                {
                    return Assembly.LoadFile(path);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException(exception.Message, exception);
                }
            }
        }
        */

        private static Assembly LoadAssembly(string path)
        {
            /*
            var domain = AppDomain.CreateDomain("H.NET.Plugins.Domain", AppDomain.CurrentDomain.Evidence, new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(path) ?? Environment.CurrentDirectory
            });

            domain.UnhandledException += (sender, args) =>
                MessageBox.Show(((Exception) args.ExceptionObject).ToString());
            */

            //var type = typeof(Proxy);
            //var value = (Proxy)domain.CreateInstanceAndUnwrap(
            //    type.Assembly.FullName,
            //    type.FullName);

            //var assembly = value.GetAssembly(path);
            //var plugin = Domain.CreateInstanceFromAndUnwrap(path, type.Name) as T;
            

            return Assembly.LoadFrom(path);
        }

        private void TryClean()
        {
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
