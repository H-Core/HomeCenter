using System;
using System.Collections.Generic;
using H.NET.Storages;
using H.NET.Storages.Extensions;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Services
{
    public class StorageService
    {
        #region Properties
        
        // TODO: options to settings?
        public CommandsStorage Storage { get; } = new CommandsStorage(Options.CompanyName);
        
        #endregion

        #region Constructors

        public StorageService() //Settings settings
        {
            Storage.Load();
        }

        #endregion

        #region Public methods

        public List<KeyValuePair<string, Command>> GetUniqueCommands() => Storage.UniqueValues(i => i.Value);

        public (string key, Command command) GetCommand(string key)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));

            if (Storage.TryGetValue(key, out var result))
            {
                return (key, result);
            }

            foreach (var pair in Storage)
            {
                var tryKey = pair.Key;
                if (!tryKey.Contains("*"))
                {
                    continue;
                }

                var subKeys = tryKey.Split('*');
                if (subKeys.Length < 2)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(subKeys[0]))
                {
                    continue;
                }

                if (key.StartsWith(subKeys[0]))
                {
                    var command = pair.Value?.Clone() as Command;
                    if (command != null)
                    {
                        var argument = key.Substring(subKeys[0].Length);
                        foreach (var line in command.Lines)
                        {
                            line.Text = line.Text.Replace("*", argument);
                        }
                    }

                    return (key, command);
                }
            }

            return (null, new Command(null, key));
        }

        public void Save() => Storage.Save();
        public void Load() => Storage.Load();

        public Command this[string key] {
            get => Storage[key];
            set => Storage[key] = value;
        }

        public bool ContainsKey(string key) => Storage.ContainsKey(key);

        public bool TryGetValue(string key, out Command value)
        {
            if (!ContainsKey(key))
            {
                value = default(Command);
                return false;
            }

            value = this[key];
            return true;
        }

        public bool Remove(string key) => Storage.Remove(key);

        #endregion
    }
}
