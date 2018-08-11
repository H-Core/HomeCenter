using System;
using System.Linq;
using H.NET.Core.Runners;
using H.NET.Utilities;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public class UiRunner : Runner
    {
        #region Properties

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

        public UiRunner()
        {
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
                ModuleManager.Instance.SetInstanceIsEnabled(name, true);
                var obj = ModuleManager.Instance.Instances.GetObject(name);
                if (obj.Exception != null)
                {
                    throw obj.Exception;
                }

                ModuleManager.RegisterHandlers();
            }, "name");
            AddAction("disable-module", name =>
            {
                ModuleManager.Instance.SetInstanceIsEnabled(name, false);
                var obj = ModuleManager.Instance.Instances.GetObject(name);
                if (obj.Exception != null)
                {
                    throw obj.Exception;
                }

                ModuleManager.RegisterHandlers();
            }, "name");

            AddInternalAction("install-assembly", command => ModuleManager.Instance.Install(command), "path");
            AddInternalAction("uninstall-assembly", command => ModuleManager.Instance.Uninstall(command), "name");
            AddInternalAction("update-assembly", command => ModuleManager.Instance.Update(command), "name");
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
                    names.Select(name => $"install-assembly {ModuleManager.Instance.AssembliesSettingsFile.Get(name).OriginalPath}"));
                arguments += ";update-restart print All modules have been updated";

                foreach (var name in names)
                {
                    Print($"Updating {name}...");
                    try
                    {
                        ModuleManager.Instance.Update(name);
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

        private static string[] GetCanBeUpdatedAssemblies() =>
            ModuleManager.Instance.AssembliesSettings.Keys.Where(ModuleManager.Instance.UpdatingIsNeed).ToArray();

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
