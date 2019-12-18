using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace H.NET.Utilities
{
    public class IpcServer : IDisposable
    {
        #region Properties

        public int Port { get; }
        private TcpListener Listener { get; set; }
        public bool IsDisposed { get; private set; }

        #endregion

        #region Constructors

        public IpcServer(int port)
        {
            Port = port;
            Listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port));

            try
            {
                Listener.Start();
                Listener.BeginAcceptTcpClient(OnClientAccepted, Listener);
            }
            catch (SocketException)
            {
                // ignore - Already started by other source
            }
        }

        #endregion

        #region Events

        public delegate void MessageDelegate(string message);
        public event MessageDelegate NewMessage;

        #endregion

        private void OnClientAccepted(IAsyncResult result)
        {
            if (!(result.AsyncState is TcpListener listener))
            {
                return;
            }

            if (IsDisposed)
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

        #region Dispose

        public void Dispose()
        {
            IsDisposed = true;
            Listener?.Stop();
            Listener?.Server?.Dispose();
            Listener = null;
        }

        #endregion
    }
}
