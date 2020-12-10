using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using H.Core;
using H.Core.Managers;
using H.Core.Runners;
using HomeCenter.NET.Runners;
using HomeCenter.NET.Services;
using HomeCenter.NET.Utilities;
using HomeCenter.NET.ViewModels;
using HomeCenter.NET.ViewModels.Modules;

namespace HomeCenter.NET.Initializers
{
    public class StaticModulesInitializer
    {
        public StaticModulesInitializer(IWindowManager windowManager, MainViewModel model, RunnerService runnerService, IpcService ipcService, BaseManager baseManager)
        {
            var _ = ipcService.StartAsync();

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

            Task Say(string? text)
            {
                //var synthesizer = moduleService.Synthesizer;
                //if (synthesizer == null)
                {
                    model.Print("Synthesizer is not found");
                    return Task.CompletedTask;
                }

                //var bytes = await synthesizer.ConvertAsync(text ?? string.Empty);

                //await bytes.PlayAsync();
            }

            Task<List<string>> Search(string? text)
            {
                //var searcher = moduleService.Searcher;
                //if (searcher == null)
                {
                    model.Print("Searcher is not found");
                    return Task.FromResult(new List<string>());
                }

                //return await searcher.Search(text ?? string.Empty);
            }

            Module.SearchFunc = Search;

            var staticRunners = new List<IRunner>
            {
                new DefaultRunner(model.Print, model.Warning, Say, Search),
                new KeyboardRunner(),
                new WindowsRunner(),
                new ClipboardRunner
                {
                    ClipboardAction = command => Application.Current.Dispatcher?.Invoke(() => Clipboard.SetText(command)),
                    ClipboardFunc = () => Application.Current.Dispatcher?.Invoke(Clipboard.GetText)
                },
                new UiRunner(ipcService, runnerService)
                {
                    // TODO: refactor
                    RestartAction = command => Application.Current.Dispatcher?.Invoke(() => Restart(command)),
                    UpdateRestartAction = command => Application.Current.Dispatcher?.Invoke(() => RestartWithUpdate(command)),
                    ShowUiAction = () => Application.Current.Dispatcher?.Invoke(() => model.IsVisible = !model.IsVisible),
                    ShowSettingsAction = () => Application.Current.Dispatcher?.Invoke(model.ShowSettingsAsync),
                    ShowCommandsAction = () => Application.Current.Dispatcher?.Invoke(model.ShowCommandsAsync),
                    ShowModuleSettingsAction = name => Application.Current.Dispatcher?.Invoke(async () => await ShowModuleSettings(name)),
                    StartRecordAction = () => Application.Current.Dispatcher?.Invoke(async () => await baseManager.StartAsync()),
                },
                new InternetRunner()
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

            Process.Start($"\"{Options.FilePath}\"", $"/restart {run} {additionalArguments}");
            Application.Current.Shutdown();
        }

        #endregion

    }
}
