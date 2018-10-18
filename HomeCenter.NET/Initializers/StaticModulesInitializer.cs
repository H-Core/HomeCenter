using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using H.NET.Core;
using H.NET.Core.Runners;
using HomeCenter.NET.Runners;
using HomeCenter.NET.Services;
using HomeCenter.NET.Utilities;
using HomeCenter.NET.ViewModels;
using HomeCenter.NET.ViewModels.Modules;

namespace HomeCenter.NET.Initializers
{
    public class StaticModulesInitializer
    {
        public StaticModulesInitializer(IWindowManager windowManager, MainViewModel model, MainService mainService, ModuleService moduleService, IpcService ipcService)
        {
            void ShowModuleSettings(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    model.Print("ShowModuleSettings: Module name is empty");
                    return;
                }

                var module = moduleService.GetPlugin<IModule>(name)?.Value;
                if (module == null)
                {
                    model.Print($"ShowModuleSettings: Module {name} is not found");
                    return;
                }

                windowManager.ShowWindow(new ModuleSettingsViewModel(module));
            }

            async Task Say(string text)
            {
                var synthesizer = moduleService.Synthesizer;
                if (synthesizer == null)
                {
                    model.Print("Synthesizer is not found");
                    return;
                }

                var bytes = await synthesizer.Convert(text);

                await bytes.PlayAsync();
            }

            async Task<List<string>> Search(string text)
            {
                var searcher = moduleService.Searcher;
                if (searcher == null)
                {
                    model.Print("Searcher is not found");
                    return new List<string>();
                }

                return await searcher.Search(text);
            }

            Runner.SearchFunc = Search;

            var staticRunners = new List<IRunner>
            {
                new DefaultRunner(model.Print, Say, Search),
                new KeyboardRunner(),
                new WindowsRunner(),
                new ClipboardRunner
                {
                    ClipboardAction = command => Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(command)),
                    ClipboardFunc = () => Application.Current.Dispatcher.Invoke(Clipboard.GetText)
                },
                new UiRunner(moduleService, ipcService)
                {
                    // TODO: refactor
                    RestartAction = command => Application.Current.Dispatcher.Invoke(() => mainService.Restart(command)),
                    UpdateRestartAction = command => Application.Current.Dispatcher.Invoke(() => mainService.RestartWithUpdate(command)),
                    ShowUiAction = () => Application.Current.Dispatcher.Invoke(() => model.IsVisible = !model.IsVisible),
                    ShowSettingsAction = () => Application.Current.Dispatcher.Invoke(model.ShowSettings),
                    ShowCommandsAction = () => Application.Current.Dispatcher.Invoke(model.ShowCommands),
                    ShowModuleSettingsAction = name => Application.Current.Dispatcher.Invoke(() => ShowModuleSettings(name)),
                    StartRecordAction = timeout => Application.Current.Dispatcher.Invoke(() => mainService.StartRecord(timeout))
                }
            };
            foreach (var runner in staticRunners)
            {
                moduleService.AddStaticInstance(runner.ShortName, runner);
            }
        }
    }
}
