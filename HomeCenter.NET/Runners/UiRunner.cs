using System;
using H.NET.Core.Runners;
using H.NET.Utilities;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public class UiRunner : Runner
    {
        #region Properties

        public Action ShowUiAction { private get; set; }
        public Action ShowSettingsAction { private get; set; }
        public Action ShowCommandsAction { private get; set; }
        public Action StartRecordAction { private get; set; }

        #endregion

        #region Constructors

        public UiRunner()
        {
            AddInternalAction("show-ui", command => ShowUiAction?.Invoke());
            AddInternalAction("show-settings", command => ShowSettingsAction?.Invoke());
            AddInternalAction("show-commands", command => ShowCommandsAction?.Invoke());
            AddInternalAction("start-record", command => StartRecordAction?.Invoke());
            AddInternalAction("deskband", DeskBandCommand);
            AddInternalAction("enable-module", command => ModuleManager.Instance.SetInstanceIsEnabled(command, true), "name");
            AddInternalAction("disable-module", command => ModuleManager.Instance.SetInstanceIsEnabled(command, false), "name");
        }

        #endregion

        #region Private methods

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
