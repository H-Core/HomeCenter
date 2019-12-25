using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Plugins.Tests.Utilities
{
    public static class ResourcesUtilities
    {
        public static Stream ReadFileAsStream(string name, Assembly assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

            return assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"\"{name}\" is not found in embedded resources");
        }

        public static string ReadFileAsString(string name, Assembly assembly = null)
        {
            using var stream = ReadFileAsStream(name, assembly);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
        
        public static byte[] ReadFileAsBytes(string name, Assembly assembly = null)
        {
            using var stream = ReadFileAsStream(name, assembly);
            using var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }
    }
}