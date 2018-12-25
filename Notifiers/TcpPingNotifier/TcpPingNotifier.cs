using System;
using System.Net.Sockets;

namespace H.NET.Notifiers
{
    public class TcpPingNotifier : TimerNotifier
    {
        #region Properties

        private string Ip { get; set; }
        private int Port { get; set; }
        private bool OnlyIfNetworkActive { get; set; }

        #endregion

        #region Contructors

        public TcpPingNotifier()
        {
            AddSetting(nameof(Ip), o => Ip = o, o => true, string.Empty);
            AddSetting(nameof(Port), o => Port = o, o => true, 0);
            AddSetting(nameof(OnlyIfNetworkActive), o => OnlyIfNetworkActive = o, o => true, true);
        }

        #endregion

        #region Protected methods

        protected override bool OnResult()
        {
            var isAvailableNetworkActive = GetVariable<bool>("$internet$", true);
            if (OnlyIfNetworkActive && !isAvailableNetworkActive)
            {
                return false;
            }

            try
            {
                using (new TcpClient(Ip, Port))
                {
                }

                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

        #endregion

    }
}
