using System.IO;
using System.Linq;

namespace H.NET.Plugins.Utilities
{
    public static class DirectoryUtilities
    {
        public static void Copy(string fromFolder, string toFolder, bool overwrite = false)
        {
            Directory
                .EnumerateFiles(fromFolder, "*.*", SearchOption.AllDirectories)
                .AsParallel()
                .ForAll(from =>
                {
                    var to = from.Replace(fromFolder, toFolder);

                    // Create directories if need
                    var toSubFolder = Path.GetDirectoryName(to);
                    if (!string.IsNullOrWhiteSpace(toSubFolder))
                    {
                        Directory.CreateDirectory(toSubFolder);
                    }

                    File.Copy(from, to, overwrite);
                });
        }

        public static string CombineAndCreateDirectory(params string[] arguments) =>
            Directory.CreateDirectory(Path.Combine(arguments)).FullName;
    }
}
