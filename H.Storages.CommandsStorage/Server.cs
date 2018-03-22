using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace H.Storages
{
    public class Server
    {
        #region Properties

        public static int Port { get; } = 19445;
        private TcpListener Listener { get; } = new TcpListener(new IPEndPoint(IPAddress.Any, Port));

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

        public Server()
        {
            Listener.Start();

            Listener.BeginAcceptTcpClient(OnClientAccepted, Listener);
        }
    }

    public static class Client
    {
        public static async Task Write(string message)
        {
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), Server.Port);

                using (var writer = new StreamWriter(client.GetStream()))
                {
                    writer.Write(message);
                }
            }
        }
    }
}
