using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using HomeCenter.NET.Properties;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET
{
    public partial class App
    {
        private Windows.MainWindow Window { get; set; }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var isKillAll = e.Args.Contains("/killall");
            if (isKillAll)
            {
                Process.GetProcessesByName(Options.ApplicationName)
                    .Where(i => i .Id != Process.GetCurrentProcess().Id)
                    .AsParallel()
                    .ForAll(i => i.Kill());
            }

            var isRestart = e.Args.Contains("/restart");

            // If current process is not first
            if (Process.GetProcessesByName(Options.ApplicationName).Length > 1 &&
                !isRestart && !isKillAll)
            {
                Current.Shutdown();
                return;
            }

            AppDomain.CurrentDomain.UnhandledException += (o, args) => OnException(args.ExceptionObject);
            Current.DispatcherUnhandledException += (o, args) =>
            {
                args.Handled = true;
                OnException(args.Exception);
            };

            Window = new Windows.MainWindow();

            if (isRestart || !Settings.Default.IsStartMinimized)
            {
                Window.Show();
            }

            await Window.Load();

            if (Settings.Default.AutoUpdateAssemblies)
            {
                Run("update-assemblies");
            }
            if (e.Args.Contains("/run"))
            {
                var commandIndex = e.Args.ToList().IndexOf("/run") + 1;
                var text = e.Args[commandIndex].Trim('"');
                var commands = text.Split(';');

                foreach (var command in commands)
                {
                    Run(command);
                }
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Window?.Dispose();
            Window = null;
        }

        private static void Run(string command) => NET.Windows.MainWindow.GlobalRun(command);

        private static void OnException(object exceptionObject)
        {
            if (!(exceptionObject is Exception exception))
            {
                exception = new NotSupportedException($"Unhandled exception doesn't derive from System.Exception: {exceptionObject}");
            }

            SafeActions.ShowException(exception);
        }
    }
}
