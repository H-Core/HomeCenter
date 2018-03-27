﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H.NET.Core.Runners;
using H.NET.Core.Storages;
using H.NET.Storages.Extensions;
using H.NET.Utilities;
using Newtonsoft.Json;

namespace H.NET.Storages
{
    public class CommandsStorage : InvariantDictionaryStorage<Command>
    {
        #region Properties

        public const string CompanyName = "VoiceActions.NET";
        public const int MaxCopiesCount = 50;
        public AppDataFile AppDataFile { get; } = new AppDataFile(CompanyName, "commands.json");

        #endregion

        #region Public methods

        public override void Save()
        {
            var uniqueCommands = this.UniqueValues(command => command.Value.Data).Select(pair => pair.Value);
            AppDataFile.FileData = JsonConvert.SerializeObject(uniqueCommands, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new GoodPropertiesOnlyResolver()
            });
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
            var _ = new AppDataFile(CompanyName, "copies", $"commands_{DateTime.Now:MM_dd_yyyy_hh_mm_ss_tt}.json")
            {
                FileData = text
            };
            
            new AppDataFolder(CompanyName, "copies")
                .Files
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .Skip(MaxCopiesCount)
                .AsParallel()
                .ForAll(File.Delete);
        } 

        #endregion
    }
}
