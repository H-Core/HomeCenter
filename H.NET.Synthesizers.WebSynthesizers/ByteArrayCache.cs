using System;
using System.IO;
using System.Linq;

namespace H.NET.Synthesizers
{
    public class ByteArrayCache
    {
        #region Properties

        public static string GlobalSubFolder { get; } = "VoiceActions.NET.Cache";
        public string SubFolder { get; }
        public string Folder { get; }

        #endregion

        #region Constructors

        public ByteArrayCache(string subFolder)
        {
            SubFolder = subFolder ?? throw new ArgumentNullException(nameof(subFolder));
            Folder = Path.Combine(Path.GetTempPath(), GlobalSubFolder, subFolder);

            Directory.CreateDirectory(Folder);
        }

        public ByteArrayCache(Type type) : this(type.Name)
        {
        }

        #endregion

        #region Public methods

        public bool Contains(string text) => File.Exists(GetPath(text));

        public byte[] this[string text] {
            get {
                var path = GetPath(text);
                if (!File.Exists(path))
                {
                    return null;
                }

                return File.ReadAllBytes(path);
            }
            set => File.WriteAllBytes(GetPath(text), value);
        }

        public void Clear() => Directory.EnumerateFiles(Folder).ToList().ForEach(File.Delete);

        public void Delete(string text) => File.Delete(GetPath(text));

        #endregion

        #region Public methods

        private string GetPath(string name) => Path.Combine(Folder, name.ToLowerInvariant() + ".cache");

        #endregion
    }
}
