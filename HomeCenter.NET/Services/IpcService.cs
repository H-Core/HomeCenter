using System;
using H.NET.Utilities;
using HomeCenter.NET.Properties;

namespace HomeCenter.NET.Services
{
    public class IpcService : IDisposable
    {
        #region Properties

        public RunnerService RunnerService { get; }
        public Settings Settings { get; }

        public IpcServer IpcServer { get; set; }

        #endregion

        #region Constructors

        public IpcService(RunnerService runnerService, Settings settings)
        {
            RunnerService = runnerService ?? throw new ArgumentNullException(nameof(runnerService));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));

            IpcServer = new IpcServer(settings.InputIpcPort);
            IpcServer.NewMessage += RunnerService.Run;
        }

        #endregion

        #region Public methods

        public async void DeskBandCommand(string command)
        {
            try
            {
                await IpcClient.Write(command, Settings.OutputIpcPort);
            }
            catch (Exception exception)
            {
                RunnerService.Run($"print {exception.Message}");
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            IpcServer?.Dispose();
            IpcServer = null;
        }

        #endregion
    }
}
