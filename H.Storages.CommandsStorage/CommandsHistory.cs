using System.Collections.Generic;
using H.Storages.Utilities;
using Newtonsoft.Json;

namespace H.Storages
{
    public static class CommandsHistory
    {
        #region Properties

        public static AppDataFile AppDataFile { get; } = new AppDataFile("VoiceActions.NET", "commands-history.json");

        #endregion

        #region Public methods

        public static void Add(string command)
        {
            var history = Load();
            history.Add(command);

            AppDataFile.FileData = JsonConvert.SerializeObject(history, Formatting.Indented);
        }

        public static List<string> Load()
        {
            var text = AppDataFile.FileData;
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }

            return JsonConvert.DeserializeObject<List<string>>(text);
        }

        #endregion
    }
}
