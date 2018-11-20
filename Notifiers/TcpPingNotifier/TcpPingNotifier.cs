using System;
using System.Linq;
using System.Net.NetworkInformation;
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

        public static bool IsAvailableNetworkActive()
        {
            return NetworkInterface.GetIsNetworkAvailable() &&
                   (from face in NetworkInterface.GetAllNetworkInterfaces()
                    where face.OperationalStatus == OperationalStatus.Up
                    where (face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) && (face.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    select face.GetIPv4Statistics()).Any(statistics => (statistics.BytesReceived > 0) && (statistics.BytesSent > 0));
        }

        protected override void OnElapsed()
        {
            // TODO: Separate Runner?
            if (OnlyIfNetworkActive && !IsAvailableNetworkActive())
            {
                return;
            }

            try
            {
                using (new TcpClient(Ip, Port))
                {
                }
            }
            catch (Exception)
            {
                OnEvent();
            }
        }

        #endregion

    }
}
