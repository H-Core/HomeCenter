﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using H.NET.Core;
using H.NET.Utilities;
using HomeCenter.NET.Properties;

namespace HomeCenter.NET.Utilities
{
    public static class Options
    {
        public static string FilePath => Assembly.GetExecutingAssembly().Location; //Application.ResourceAssembly.Location
        public const string ApplicationName = "HomeCenter.NET";
        public const string CompanyName = "HomeCenter.NET";
        public const int IpcPortToHomeCenter = 19445;
        public const int IpcPortToDeskBand = 19446;

        public static Keys RecordKey => Hook.FromString(Settings.Default.RecordKey);

        public static List<string> HookIgnoredApps
        {
            get => Settings.Default.HookIgnoredApps.Split(';').Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
            set => Settings.Default.HookIgnoredApps = string.Join(";", value);
        }

        public static bool IsIgnoredApplication()
        {
            try
            {
                if (User32Utilities.AreApplicationFullScreen())
                {
                    return true;
                }

                var process = User32Utilities.GetForegroundProcess();
                var appExePath = process.MainModule.FileName;
                
                //var appProcessName = process.ProcessName;
                //var appExeName = appExePath.Substring(appExePath.LastIndexOf(@"\") + 1);

                return HookIgnoredApps.Contains(appExePath);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
