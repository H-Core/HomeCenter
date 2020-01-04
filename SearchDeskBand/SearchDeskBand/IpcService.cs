using System;
using NamedPipeWrapper;

namespace H.NET.SearchDeskBand
{
    public static class IpcService
    {
        private static NamedPipeServer<string> NamedPipeServer { get; } = new NamedPipeServer<string>("H.NET.DeskBand");
        private static NamedPipeClient<string> NamedPipeClient { get; } = new NamedPipeClient<string>("H.NET.MainApplication");

        static IpcService()
        {
            NamedPipeServer.ClientMessage += OnNewMessage;

            NamedPipeServer.Start();
            NamedPipeClient.Start();
        }

        private static void OnNewMessage(NamedPipeConnection<string, string> connection, string message)
        {
            Message?.Invoke(null, message);
        }

        public static void SendMessage(string message)
        {
            NamedPipeClient.PushMessage(message);
        }

        public static event EventHandler<string> Message;
    }
}
