using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace H.Utilities
{
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