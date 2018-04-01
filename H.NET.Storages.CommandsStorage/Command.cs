using System;
using System.Collections.Generic;
using System.Linq;

namespace H.NET.Storages
{
    public class Command
    {
        #region Properties

        public List<SingleCommand> Lines { get; set; } = new List<SingleCommand>();
        public List<SingleKey> Keys { get; set; } = new List<SingleKey>();
        public string HotKey { get; set; }

        public string KeysString => string.Join(Environment.NewLine, Keys);

        public string Data => string.Join(Environment.NewLine, Lines);

        #endregion

        #region Constructors

        public Command()
        {
        }

        public Command(List<string> keys)
        {
            Keys = keys.Select(text => new SingleKey(text)).ToList();
        }

        public Command(List<string> keys, List<string> dataLines, string hotKey = null) : this(keys)
        {
            Lines = dataLines.Select(text => new SingleCommand(text)).ToList();
            HotKey = hotKey;
        }

        public Command(string key) : this(new List<string> { key })
        {
        }

        public Command(string key, string data, string hotKey = null) : this(key)
        {
            Lines.Add(new SingleCommand(data));
            HotKey = hotKey;
        }

        #endregion

        #region ICloneable

        public object Clone() => new Command(
            Keys.Select(key => key.Text).ToList(),
            Lines.Select(command => command.Text).ToList(),
            HotKey);

        #endregion
    }
}
