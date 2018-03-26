using System;
using System.Collections.Generic;
using System.IO;

namespace H.NET.Utilities
{
    public class SpecialFolder
    {
        #region Properties

        public Environment.SpecialFolder EnvironmentSpecialFolder { get; }

        public string SubPath { get; }

        public string SpecialFolderPath => Environment.GetFolderPath(EnvironmentSpecialFolder);

        public string Folder => Directory.CreateDirectory(
                Path.Combine(SpecialFolderPath, SubPath))
            .FullName;

        public IEnumerable<string> Files => Directory.EnumerateFiles(Folder, "*.*");

        #endregion

        #region Constructors

        public SpecialFolder(Environment.SpecialFolder specialFolder, params string[] needToCombinedPathStrings)
        {
            EnvironmentSpecialFolder = specialFolder;
            SubPath = Path.Combine(needToCombinedPathStrings);
        }

        #endregion
    }
}