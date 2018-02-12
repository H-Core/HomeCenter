using System;
using System.Collections.Generic;
using System.Linq;
using HomeCenter.NET.Extensions;
using HomeCenter.NET.Utilities;
using Newtonsoft.Json;
using VoiceActions.NET.Storages;

namespace HomeCenter.NET.Storages
{
    public class CommandsStorage : InvariantDictionaryStorage<Command>
    {
        #region Properties

        public AppDataFile AppDataFile { get; } = new AppDataFile("VoiceActions.NET", "commands.json");

        #endregion

        #region Public methods

        public override void Save()
        {
            var uniqueCommands = this.UniqueValues(command => command.Value.Data).Select(pair => pair.Value);
            AppDataFile.FileData = JsonConvert.SerializeObject(uniqueCommands);
        }

        public override void Load()
        {
            Clear();

            var text = AppDataFile.FileData;
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var commands = JsonConvert.DeserializeObject<List<Command>>(text);
            foreach (var command in commands)
            {
                foreach (var key in command.Keys)
                {
                    this[key] = command;
                }
            }

            CreateCopy(text);
        }

        public void CreateCopy(string text)
        {
            var _ = new AppDataFile("VoiceActions.NET", "copies", $"commands_{DateTime.Now:MM_dd_yyyy_hh_mm_ss_tt}.json")
            {
                FileData = text
            };
        } 

        #endregion
    }
}
