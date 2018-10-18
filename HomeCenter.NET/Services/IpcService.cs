using System;
using H.NET.Utilities;
using HomeCenter.NET.Properties;

namespace HomeCenter.NET.Services
{
    public class IpcService : IDisposable
    {
        #region Properties

        public MainService MainService { get; }
        public Settings Settings { get; }

        public IpcServer IpcServer { get; set; }

        #endregion

        #region Constructors

        public IpcService(MainService mainService, Settings settings)
        {
            MainService = mainService ?? throw new ArgumentNullException(nameof(mainService));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));

            IpcServer = new IpcServer(settings.InputIpcPort);
            IpcServer.NewMessage += mainService.Run;
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
                MainService.Run($"print {exception.Message}");
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
