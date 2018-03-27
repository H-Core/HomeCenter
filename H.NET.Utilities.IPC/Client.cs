using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace H.NET.Utilities
{
    public class Client
    {
        #region Static methods

        public static async Task Write(string message, int port) => await new Client(port).Write(message);

        #endregion

        #region Properties

        public int Port { get; }

        #endregion

        #region Constructors

        public Client(int port)
        {
            Port = port;
        }

        #endregion

        #region Public methods

        public async Task Write(string message)
        {
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), Port);

                using (var writer = new StreamWriter(client.GetStream()))
                {
                    writer.Write(message);
                }
            }
        }

        #endregion
    }
}