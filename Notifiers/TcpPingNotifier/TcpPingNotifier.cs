using System;
using System.Net.Sockets;

namespace H.NET.Notifiers
{
    public class TcpPingNotifier : TimerNotifier
    {
        #region Properties

        private string Ip { get; set; }
        private int Port { get; set; }

        #endregion

        #region Contructors

        public TcpPingNotifier()
        {
            AddSetting(nameof(Ip), o => Ip = o, o => true, string.Empty);
            AddSetting(nameof(Port), o => Port = o, o => true, 0);
        }

        #endregion

        #region Protected methods

        protected override void OnElapsed()
        {
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
