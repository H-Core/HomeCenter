using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using H.NET.Core;
using H.NET.Utilities.Plugins;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Services;
using HomeCenter.NET.Utilities;
using HomeCenter.NET.ViewModels;

namespace HomeCenter.NET.Initializers
{
    public static class InitializeHelper
    {
        public static async Task InitializeDynamicModules(RunnerService runnerService, HookService hookService, ModuleService moduleService, MainViewModel model)
        {
            AssembliesManager.LogAction = model.Print;
            Module.LogAction = model.Print;

            model.Print("Loading modules...");
            try
            {
                await Task.Run(moduleService.Load);

                moduleService.AddUniqueInstancesIfNeed();
                moduleService.RegisterHandlers(runnerService);
                moduleService.UpdateActiveModules();

                hookService.UpdateCombinations();

                model.Print("Loaded");
            }
            catch (Exception exception)
            {
                model.Print(exception.ToString());
            }
        }

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

        public static void CheckUpdate(string[] args, RunnerService runnerService)
        {
            var isUpdating = args.Contains("/updating");
            if (!isUpdating && Settings.Default.AutoUpdateAssemblies)
            {
                runnerService.Run("update-assemblies");
            }
        }

        public static void CheckRun(string[] args, RunnerService runnerService)
        {
            if (args.Contains("/run"))
            {
                var commandIndex = args.ToList().IndexOf("/run") + 1;
                var text = args[commandIndex].Trim('"');
                var commands = text.Split(';');

                foreach (var command in commands)
                {
                    runnerService.Run(command);
                }
            }
        }
    }
}
