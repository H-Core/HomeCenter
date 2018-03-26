using System;
using System.IO;
using H.NET.Core.Utilities;
using Microsoft.Win32;

namespace HomeCenter.NET.Utilities
{
    public static class Startup
    {
        private static string KeyName { get; } = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static void Set(string fileName, bool autoStart, string arguments = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var name = Path.GetFileName(fileName);
            if (name == null)
            {
                throw new Exception("File name is not correct");
            }

            using (var localMachine = Registry.CurrentUser)
            using (var key = localMachine.OpenSubKey(KeyName, true) ?? localMachine.CreateSubKey(KeyName))
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

        public static bool IsStartup(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var name = Path.GetFileName(fileName);
            if (name == null)
            {
                throw new Exception("File name is not correct");
            }

            using (var localMachine = Registry.CurrentUser)
            using (var key = localMachine.OpenSubKey(KeyName))
            {
                if (!(key?.GetValue(name) is string value))
                {
                    return false;
                }

                var valueFileName = ToFileName(value);
                if (!Compare(fileName, valueFileName))
                {
                    return false;
                }
            }

            return true;
        }

        #region Private methods

        private static string ToValue(string fileName, string arguments) => $"\"{fileName ?? ""}\" {arguments ?? ""}".Trim();

        private static string ToFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim().SplitOnlyFirstIgnoreQuote(' ')[0];
        }

        private static bool Compare(string first, string second) => 
            string.Equals(first.Trim(' ', '\"'), second.Trim(' ', '\"'), StringComparison.OrdinalIgnoreCase);

        #endregion

    }
}