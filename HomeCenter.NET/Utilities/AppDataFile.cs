using System;
using System.IO;

namespace HomeCenter.NET.Utilities
{
    public class AppDataFile
    {
        #region Properties

        public string SubPath { get; }
        public string SubDirectory => Path.GetDirectoryName(SubPath);
        public string FileName => Path.GetFileName(SubPath);

        public string AppDataFolder => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public string Folder => Directory.CreateDirectory(
            Path.Combine(AppDataFolder, SubDirectory))
            .FullName;

        public string FullPath => Path.Combine(Folder, FileName);

        public string FileData
        {
            get => File.Exists(FullPath) ? File.ReadAllText(FullPath) : null;
            set => File.WriteAllText(FullPath, value);
        }

        #endregion

        #region Constructors

        public AppDataFile(params string[] needToCombinedPathStrings)
        {
            SubPath = Path.Combine(needToCombinedPathStrings);
        }

        #endregion
    }
}
