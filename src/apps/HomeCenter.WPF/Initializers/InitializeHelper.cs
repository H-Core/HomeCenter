using System.Diagnostics;
using System.Linq;
using System.Windows;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Initializers
{
    public static class InitializeHelper
    {
        public static void CheckKillAll(string[] args)
        {
            var isKillAll = args.Contains("/killall");
            if (isKillAll)
            {
                Process.GetProcessesByName(Options.ApplicationName)
                    .Where(i => i.Id != Process.GetCurrentProcess().Id)
                    .AsParallel()
                    .ForAll(i => i.Kill());
            }
        }

        public static void CheckNotFirstProcess(string[] args)
        {
            var isKillAll = args.Contains("/killall");
            var isRestart = args.Contains("/restart");
            if (Process.GetProcessesByName(Options.ApplicationName).Length > 1 &&
                !isRestart && !isKillAll)
            {
                Application.Current.Shutdown();
            }
        }

        public static void CheckUpdate(string[] args)
        {
            var isUpdating = args.Contains("/updating");
            if (!isUpdating && Settings.Default.AutoUpdateAssemblies)
            {
                //runnerService.Run("update-assemblies");
            }
        }

        public static void CheckRun(string[] args)
        {
            if (args.Contains("/run"))
            {
                var commandIndex = args.ToList().IndexOf("/run") + 1;
                var text = args[commandIndex].Trim('"');
                var commands = text.Split(';');

                foreach (var command in commands)
                {
                    //runnerService.Run(command);
                }
            }
        }
    }
}
