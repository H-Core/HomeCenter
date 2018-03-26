using System.IO;
using System.Linq;

namespace H.NET.Plugins.Utilities
{
    public static class DirectoryUtilities
    {
        public static void CopyDirectory(string fromFolder, string toFolder) => Directory
            .GetFiles(fromFolder, "*.*", SearchOption.AllDirectories)
            .ToList()
            .ForEach(fromPath =>
            {
                var toPath = fromPath.Replace(fromFolder, toFolder);
                Directory.CreateDirectory(Path.GetDirectoryName(toPath) ?? "");

                File.Copy(fromPath, toPath, true);
            });

        public static string CombineAndCreateDirectory(params string[] arguments) =>
            Directory.CreateDirectory(Path.Combine(arguments)).FullName;
    }
}
