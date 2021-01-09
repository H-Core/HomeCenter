using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using H.Core.Runners;
using HomeCenter.NET.Runners;
using HomeCenter.NET.Utilities;
using HomeCenter.NET.ViewModels;

namespace HomeCenter.NET.Initializers
{
    public class StaticModulesInitializer
    {
        public StaticModulesInitializer(MainViewModel model)
        {
            Task ShowModuleSettings(string? name)
            {
                if (name == null || string.IsNullOrWhiteSpace(name))
                {
                    model.Print("ShowModuleSettings: Module name is empty");
                    return Task.CompletedTask;
                }

                //var module = moduleService.GetPlugin<IModule>(name)?.Value;
                //if (module == null)
                {
                    model.Print($"ShowModuleSettings: Module {name} is not found");
                    return Task.CompletedTask;
                }

                //await windowManager.ShowWindowAsync(new ModuleSettingsViewModel(module));
            }

            var staticRunners = new List<IRunner>
            {
                new UiRunner
                {
                    // TODO: refactor
                    RestartAction = command => Application.Current.Dispatcher?.Invoke(() => Restart(command)),
                    UpdateRestartAction = command => Application.Current.Dispatcher?.Invoke(() => RestartWithUpdate(command)),
                    ShowUiAction = () => Application.Current.Dispatcher?.Invoke(() => model.IsVisible = !model.IsVisible),
                    ShowSettingsAction = () => Application.Current.Dispatcher?.Invoke(model.ShowSettingsAsync),
                    ShowCommandsAction = () => Application.Current.Dispatcher?.Invoke(model.ShowCommandsAsync),
                    ShowModuleSettingsAction = name => Application.Current.Dispatcher?.Invoke(async () => await ShowModuleSettings(name)),
                    //StartRecordAction = () => Application.Current.Dispatcher?.Invoke(async () => await baseManager.StartAsync()),
                },
            };
            foreach (var runner in staticRunners)
            {
                //moduleService.AddStaticInstance(runner.ShortName, runner);
            }
        }

        #region Restart

        public static void RestartWithUpdate(string? command) => Restart(command, "/updating");

        public static void Restart(string? command, string? additionalArguments = null)
        {
            if (command == null || string.IsNullOrWhiteSpace(command))
            {
                Restart(new List<string>(), additionalArguments);
            }
            else
            {
                Restart(new[] { command }, additionalArguments);
            }
        }

        public static void Restart(ICollection<string> commands, string? additionalArguments = null)
        {
            var run = commands.Any() ? $"/run \"{string.Join(";", commands)}\"" : string.Empty;

            System.Diagnostics.Process.Start($"\"{Options.FilePath}\"", $"/restart {run} {additionalArguments}");
            Application.Current.Shutdown();
        }

        #endregion

    }
}
