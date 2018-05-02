using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace H.NET.Utilities
{
    public class IpcServer
    {
        #region Properties

        public bool IsEnabled { get; private set; }
        public int Port { get; }
        private TcpListener Listener { get; }

        #endregion

        #region Constructors

        public IpcServer(int port)
        {
            Port = port;
            Listener = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
            Listener.Start();
            Listener.BeginAcceptTcpClient(OnClientAccepted, Listener);

            IsEnabled = true;
        }

        #endregion

        #region Events

        public delegate void MessageDelegate(string message);
        public event MessageDelegate NewMessage;

        #endregion

        public void Stop()
        {
            IsEnabled = false;
            Listener.Stop();
        }

        private void OnClientAccepted(IAsyncResult result)
        {
            if (!(result.AsyncState is TcpListener listener))
            {
                return;
            }

            if (!IsEnabled)
            {
                return;
            }
            
            try
            {
                using (var client = listener.EndAcceptTcpClient(result))
                using (var reader = new StreamReader(client.GetStream()))
                {
                    NewMessage?.Invoke(reader.ReadToEnd());
                }
            }
            finally
            {
                listener.BeginAcceptTcpClient(OnClientAccepted, listener);
            }
        }
    }
}
