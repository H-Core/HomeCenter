using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core.Runners;
using H.NET.Core.Utilities;

namespace HomeCenter.NET.Runners
{
    public class DefaultRunner : Runner
    {
        #region Properties
        
        private string UserName { get; set; }
        
        #endregion

        #region Constructors

        public DefaultRunner()
        {
            AddAction("say", Say, "text");
            AddAction("print", Print, "text");
            AddInternalAction("run", Run, "other_command_key");

            AddAsyncAction("sleep", SleepCommand, "integer");
            AddAction("sync-sleep", command => Thread.Sleep(int.TryParse(command, out var result) ? result : 1000), "integer");

            AddAction("start", StartCommand, "program.exe arguments");

            AddAction("say-my-name", async command =>
            {
                if (string.IsNullOrWhiteSpace(UserName))
                {
                    Say("Я еще не знаю вашего имени. Скажите ваше имя");

                    var name = await WaitNextCommand(10000);
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        return;
                    }

                    // First char to upper case
                    name = name[0].ToString().ToUpper() + name.Substring(1);

                    Settings.CopyFrom("username", name);
                }

                Say($"Ваше имя {UserName}");
            });
            AddSetting("username", o => UserName = o, NoEmpty, string.Empty);

            AddVariable("$username$", () => UserName);
        }

        #endregion

        #region Private methods

        private static async Task SleepCommand(string command)
        {
            var delay = int.TryParse(command, out var result) ? result : 1000;
            await Task.Delay(delay);
        }

        private static void StartCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            var values = command.SplitOnlyFirstIgnoreQuote(' ');

            var path = values[0].Trim('\"', '\\').Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("\\", "/");
            Process.Start(path, values[1]);
        }

        #endregion
    }
}
