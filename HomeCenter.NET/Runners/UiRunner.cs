using System;
using System.Linq;
using H.NET.Core.Runners;
using H.NET.Utilities;
using HomeCenter.NET.Extensions;
using HomeCenter.NET.Services;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public class UiRunner : Runner
    {
        #region Properties

        public ModuleService ModuleService { get; }

        private const int DefaultRecordTimeout = 10000;

        public Action<string> RestartAction { private get; set; }
        public Action<string> UpdateRestartAction { private get; set; }
        public Action ShowUiAction { private get; set; }
        public Action ShowSettingsAction { private get; set; }
        public Action ShowCommandsAction { private get; set; }
        public Action<string> ShowModuleSettingsAction { private get; set; }
        public Action<int> StartRecordAction { private get; set; }

        #endregion

        #region Constructors

        public UiRunner(ModuleService moduleService)
        {
            ModuleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));

            AddInternalAction("restart", command => RestartAction?.Invoke(command));
            AddInternalAction("update-restart", command => UpdateRestartAction?.Invoke(command));
            AddInternalAction("show-ui", command => ShowUiAction?.Invoke());
            AddInternalAction("show-settings", command => ShowSettingsAction?.Invoke());
            AddInternalAction("show-commands", command => ShowCommandsAction?.Invoke());
            AddInternalAction("show-module-settings", command => ShowModuleSettingsAction?.Invoke(command), "name");
            AddInternalAction("start-record", command => StartRecordAction?.Invoke(int.TryParse(command, out var result) ? result : DefaultRecordTimeout));
            AddInternalAction("deskband", DeskBandCommand);
            AddAction("enable-module", name =>
            {
                moduleService.SetInstanceIsEnabled(name, true);
                var obj = moduleService.Instances.GetObject(name);
                if (obj.Exception != null)
                {
                    throw obj.Exception;
                }

                moduleService.RegisterHandlers();
            }, "name");
            AddAction("disable-module", name =>
            {
                moduleService.SetInstanceIsEnabled(name, false);
                var obj = moduleService.Instances.GetObject(name);
                if (obj.Exception != null)
                {
                    throw obj.Exception;
                }

                moduleService.RegisterHandlers();
            }, "name");

            AddInternalAction("install-assembly", command => this.CheckPathAndRun(command, moduleService.Install), "path");
            AddInternalAction("uninstall-assembly", command => this.CheckPathAndRun(command, moduleService.Uninstall), "name");
            AddInternalAction("update-assembly", command => this.CheckPathAndRun(command, moduleService.Update), "name");
            AddInternalAction("check-assemblies-updates", command =>
            {
                var names = GetCanBeUpdatedAssemblies();
                if (!names.Any())
                {
                    Print("All modules already updated");
                    return;
                }

                foreach (var name in names)
                {
                    Print($"Assembly {name} can be updated");
                }
            });
            AddInternalAction("update-assemblies", command =>
            {
                Print("Checking updates...");
                var names = GetCanBeUpdatedAssemblies();
                if (!names.Any())
                {
                    Print("All modules already updated");
                    return;
                }

                var arguments = string.Join(";", 
                    names.Select(name => $"install-assembly {moduleService.AssembliesSettingsFile.Get(name).OriginalPath}"));
                arguments += ";update-restart print All modules have been updated";

                foreach (var name in names)
                {
                    Print($"Updating {name}...");
                    try
                    {
                        moduleService.Update(name);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                Run($"update-restart {arguments}");
            });
        }

        #endregion

        #region Private methods

        private string[] GetCanBeUpdatedAssemblies() =>
            ModuleService.AssembliesSettings.Keys.Where(ModuleService.UpdatingIsNeed).ToArray();

        private static async void DeskBandCommand(string command)
        {
            try
            {
                await IpcClient.Write(command, Options.IpcPortToDeskBand);
            }
            catch (Exception exception)
            {
                Log(exception.Message);
            }
        }

        #endregion
    }
}
