using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HomeCenter.NET.Utilities
{
    public class Command : ICloneable
    {
        #region Properties

        public List<SingleCommand> Commands { get; set; } = new List<SingleCommand>();
        public List<SingleKey> Keys { get; set; } = new List<SingleKey>();

        [JsonIgnore]
        public string KeysString => string.Join(Environment.NewLine, Keys);

        [JsonIgnore]
        public string Data => string.Join(Environment.NewLine, Commands);

        #endregion

        #region Constructors

        public Command()
        {
        }

        public Command(List<string> keys)
        {
            Keys = keys.Select(text => new SingleKey(text)).ToList();
        }

        public Command(List<string> keys, List<string> dataLines) : this(keys)
        {
            Commands = dataLines.Select(text => new SingleCommand(text)).ToList();
        }

        public Command(string key) : this(new List<string> { key })
        {
        }

        public Command(string key, string data) : this(key)
        {
            Commands.Add(new SingleCommand(data));
        }

        #endregion

        #region ICloneable

        public object Clone() => new Command(
            Keys.Select(key => key.Text).ToList(),
            Commands.Select(command => command.Text).ToList());

        #endregion
    }
}
