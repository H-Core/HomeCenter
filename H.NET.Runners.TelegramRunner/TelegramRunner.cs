using System;
using H.NET.Core.Runners;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace H.NET.Runners
{
    public class TelegramRunner : SimpleRunner
    {
        #region Properties

        private int _userId;
        public int UserId
        {
            get => _userId;
            set {
                _userId = value;

                if (!UsedIdIsValid(value))
                {
                    return;
                }

                ChatId = new ChatId(value);
            }
        }

        private string _token;
        public string Token
        {
            get => _token;
            set {
                _token = value;

                if (!TokenIsValid(value))
                {
                    return;
                }

                Client = new TelegramBotClient(value);
            }
        }

        private TelegramBotClient Client { get; set; }
        private ChatId ChatId { get; set; }

        #endregion

        #region Constructors

        public TelegramRunner()
        {
            AddSetting("Token", o => Token = o, TokenIsValid, string.Empty);
            AddSetting("UserId", o => UserId = o, UsedIdIsValid, 0);

            AddAction("telegram", SendMessage, "text");
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

        #region Protected methods

        public async void SendMessage(string text) => 
            await Client.SendTextMessageAsync(ChatId, text);

        #endregion
    }
}
