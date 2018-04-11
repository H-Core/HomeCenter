using System;
using System.IO;
using Microsoft.Win32;

namespace HomeCenter.NET.Utilities
{
    public static class Startup
    {
        private static string KeyName { get; } = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static void Set(string fileName, bool autoStart, string arguments = null)
        {
            var name = ToName(fileName);

            using (var mainKey = Registry.CurrentUser)
            using (var key = mainKey.CreateSubKey(KeyName))
            {
                if (key == null)
                {
                    throw new Exception($"Cannot open/create reg key: {KeyName}");
                }

                if (autoStart)
                {
                    var value = ToValue(fileName, arguments);
                    key.SetValue(name, value);
                }
                else
                {
                    key.DeleteValue(name);
                }
            }
        }

        public static string GetFilePath(string fileName)
        {
            var name = ToName(fileName);

            using (var mainKey = Registry.CurrentUser)
            using (var key = mainKey.OpenSubKey(KeyName))
            {
                if (!(key?.GetValue(name) is string value))
                {
                    return null;
                }

                return value;
            }
        }

        public static bool IsStartup(string fileName)
        {
            var path = GetFilePath(fileName);
            var valueFileName = ToFileName(path);

            return Compare(fileName, valueFileName);
        }

        #region Private methods

        private static string ToName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var name = Path.GetFileNameWithoutExtension(fileName);
            if (name == null)
            {
                throw new Exception("File name is not correct");
            }

            return name;
        }

        private static string ToValue(string fileName, string arguments) => $"\"{fileName}\" {arguments}".Trim();

        private static string ToFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var values = value.Trim().Split(new[] {'\"'}, StringSplitOptions.RemoveEmptyEntries);

            return values.Length > 0 ? values[0] : null;
        }

        private static bool Compare(string first, string second) => 
            string.Equals(first.Trim(' ', '\"'), second.Trim(' ', '\"'), StringComparison.OrdinalIgnoreCase);

        #endregion

    }
}