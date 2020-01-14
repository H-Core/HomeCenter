using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace H.NET.Utilities.Plugins.Extensions
{
    public static class AssemblyExtensions
    {
        public static Type[] GetTypesOfInterface(this Assembly assembly, Type type) =>
            assembly.GetExportedTypes().Where(type.IsAssignableFrom).ToArray();

        public static Type[] GetTypesOfInterface<T>(this Assembly assembly) =>
            assembly.GetTypesOfInterface(typeof(T));

        public static T[] GetObjectsOfInterface<T>(this Assembly assembly) =>
            assembly.GetTypesOfInterface<T>().Select(Activator.CreateInstance).Cast<T>().ToArray();

        public static string GetFolder(this Assembly assembly) => Path.GetDirectoryName(assembly.Location) ?? ".";

        public static string GetSimpleName(this Assembly assembly) => assembly.GetName().Name;

        public static string[] GetDllPaths(this Assembly assembly, int level = 0)
        {
            var list = new List<string> { assembly.Location };

            var folder = assembly.GetFolder();
            var assemblyNames = assembly.GetReferencedAssemblies();
            var references = assemblyNames
                .Select(assemblyName => assemblyName.CodeBase?.Replace(@"file:///", string.Empty) ?? Path.Combine(folder, assemblyName.Name + ".dll"))
                .Where(i => !Path.GetFileName(i).StartsWith("System.") &&
                            !Path.GetFileName(i).StartsWith("mscorlib") &&
                            !Path.GetFileName(i).StartsWith("netstandard"))
                .Where(File.Exists)
                .ToArray();
            
            list.AddRange(references);
            list.AddRange(references
                .Select(Assembly.LoadFrom)
                .SelectMany(i => GetDllPaths(i, level + 1)));

            // TODO: rough fix
            if (level == 0)
            {
                // Emgu.CV and other wrapper fix
                list.AddRange(GetFilesIfExists(Path.Combine(folder, "x86")));
                list.AddRange(GetFilesIfExists(Path.Combine(folder, "x64")));
            }

            list = list.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            return list.ToArray();
        }

        private static string[] GetFilesIfExists(string folder) =>
            Directory.Exists(folder) 
                ? Directory.EnumerateFiles(folder).ToArray()
                : new string[0];
    }
}
