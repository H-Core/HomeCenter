using System;
using System.IO;

namespace HomeCenter.NET.Utilities
{
    public class CommandsStorage
    {
        #region Public static properties

        public static string CompanyName { get; } = "VoiceActions.NET";

        public static string Folder => Directory.CreateDirectory(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CompanyName))
            .FullName;

        public static string FullPath => Path.Combine(Folder, "commands.json");

        /// <summary>
        /// Property for simply access to Save/Load methods
        /// </summary>
        public static string Data { get => Load(); set => Save(value); }

        #endregion

        #region Public static methods

        public static void Save(string data) => File.WriteAllText(FullPath, data);
        public static string Load() => File.Exists(FullPath) ? File.ReadAllText(FullPath) : null;

        #endregion
    }
}
