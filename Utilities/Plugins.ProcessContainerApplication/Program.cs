using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using H.Pipes;

namespace Plugins.ProcessContainerApplication
{
    internal static class Program
    {
        private static bool IsStopped { get; set; }

        [MTAThread]
        private static async Task Main(string[] arguments)
        {
            var name = arguments.FirstOrDefault() ?? "H.MainApplication";
            var client = new PipeClient<string>(name);
            client.MessageReceived += (sender, args) =>
            {
                OnMessageReceived(args.Message);
            };
            client.ExceptionOccurred += (sender, args) =>
            {
                Trace.WriteLine($"Exception: {args.Exception}");
            };
            await client.ConnectAsync();

            while (!IsStopped)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }
        }

        private static void OnMessageReceived(string message)
        {
            if (message == "stop")
            {
                IsStopped = true;
            }
        }
    }
}
