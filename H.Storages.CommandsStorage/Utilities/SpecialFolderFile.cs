using System;
using System.IO;
using System.Linq;

namespace H.Storages.Utilities
{
    public class SpecialFolderFile : SpecialFolder
    {
        #region Properties
        
        public string FileName { get; }
        public string FullPath => Path.Combine(Folder, FileName);

        public string FileData
        {
            get => File.Exists(FullPath) ? File.ReadAllText(FullPath) : null;
            set => File.WriteAllText(FullPath, value);
        }

        #endregion

        #region Constructors

        public SpecialFolderFile(Environment.SpecialFolder specialFolder, params string[] needToCombinedPathStrings) : 
            base(specialFolder, needToCombinedPathStrings.Reverse().Skip(1).Reverse().ToArray())
        {
            FileName = needToCombinedPathStrings.Any()
                ? needToCombinedPathStrings[needToCombinedPathStrings.Length - 1]
                : throw new ArgumentException(@"Need one or more values", nameof(needToCombinedPathStrings));
        }

        #endregion
    }
}