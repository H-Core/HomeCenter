using System;
using System.Net;
using System.Threading.Tasks;
using H.NET.Core.Runners;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace H.NET.Runners
{
    public class TelegramRunner : Runner
    {
        #region Properties

        public int UserId { get; set; }
        public string Token { get; set; }
        public string ProxyIp { get; set; }
        public int ProxyPort { get; set; }

        #endregion

        #region Constructors

        public TelegramRunner()
        {
            AddSetting(nameof(Token), o => Token = o, TokenIsValid, string.Empty);
            AddSetting(nameof(UserId), o => UserId = o, UsedIdIsValid, 0);
            AddSetting(nameof(ProxyIp), o => ProxyIp = o, null, string.Empty);
            AddSetting(nameof(ProxyPort), o => ProxyPort = o, null, 0);
                                                                  
            AddAsyncAction("telegram", SendMessage, "text");
        }

        public static bool TokenIsValid(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            try
            {
                var unused = new TelegramBotClient(token);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UsedIdIsValid(int usedId) => usedId > 0;

        #endregion

        #region Private methods

        private async Task SendMessage(string text)
        {
            var isProxy = !string.IsNullOrWhiteSpace(ProxyIp) && Positive(ProxyPort);
            var client = isProxy
                ? new TelegramBotClient(Token, new WebProxy(ProxyIp, ProxyPort))
                : new TelegramBotClient(Token);
            
            await client.SendTextMessageAsync(new ChatId(UserId), text);

        }

        #endregion
    }
}
