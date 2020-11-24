using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H.NET.Core.Storages;
using H.NET.Storages.Extensions;
using H.NET.Utilities;
using Newtonsoft.Json;

namespace H.NET.Storages
{
    public class CommandsStorage : InvariantDictionaryStorage<Command>
    {
        #region Properties

        public const string FileName = "Commands.json";
        public const string CopiesSubFolder = "Copies";

        public string CompanyName { get; }
        public int MaxCopiesCount { get; }
        public AppDataFile AppDataFile { get; }

        #endregion

        #region Constructors

        public CommandsStorage(string companyName, int maxCopiesCount = 50)
        {
            CompanyName = companyName;
            MaxCopiesCount = maxCopiesCount;
            AppDataFile = new AppDataFile(CompanyName, FileName);
        }

        #endregion

        #region Public methods

        public override void Save()
        {
            var uniqueCommands = this.UniqueValues(command => string.Concat(command.Value.Lines)).Select(pair => pair.Value);
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
            var _ = new AppDataFile(CompanyName, CopiesSubFolder, 
                $"{Path.GetFileNameWithoutExtension(FileName)}_{DateTime.Now:MM_dd_yyyy_hh_mm_ss_tt}{Path.GetExtension(FileName)}")
            {
                FileData = text
            };
            
            new AppDataFolder(CompanyName, CopiesSubFolder)
                .Files
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .Skip(MaxCopiesCount)
                .AsParallel()
                .ForAll(File.Delete);
        } 

        #endregion
    }
}
