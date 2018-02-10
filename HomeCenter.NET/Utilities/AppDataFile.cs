using System;
using System.IO;

namespace HomeCenter.NET.Utilities
{
    public class AppDataFile
    {
        #region Properties

        public string CompanyName { get; }
        public string FileName { get; }

        public string Folder => Directory.CreateDirectory(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CompanyName))
            .FullName;

        public string FullPath => Path.Combine(Folder, FileName);

        public string FileData
        {
            get => File.Exists(FullPath) ? File.ReadAllText(FullPath) : null;
            set => File.WriteAllText(FullPath, value);
        }

        #endregion

        #region Constructors

        public AppDataFile(string companyName, string fileName)
        {
            CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        #endregion
    }
}
