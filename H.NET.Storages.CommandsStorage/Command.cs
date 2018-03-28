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

        public Command(List<string> keys, List<string> dataLines) : this(keys)
        {
            Lines = dataLines.Select(text => new SingleCommand(text)).ToList();
        }

        public Command(string key) : this(new List<string> { key })
        {
        }

        public Command(string key, string data) : this(key)
        {
            Lines.Add(new SingleCommand(data));
        }

        #endregion

        #region ICloneable

        public object Clone() => new Command(
            Keys.Select(key => key.Text).ToList(),
            Lines.Select(command => command.Text).ToList());

        #endregion
    }
}
