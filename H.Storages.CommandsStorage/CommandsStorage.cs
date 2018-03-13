using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H.Storages.Extensions;
using H.Storages.Utilities;
using Newtonsoft.Json;
using VoiceActions.NET.Storages;

namespace H.Storages
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
            AppDataFile.FileData = JsonConvert.SerializeObject(uniqueCommands, Formatting.Indented);
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
                    this[key.Text] = command;
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

            const int maxCount = 50;
            new AppDataFolder("VoiceActions.NET", "copies")
                .Files
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .Skip(maxCount)
                .AsParallel()
                .ForAll(File.Delete);
        } 

        #endregion
    }
}
