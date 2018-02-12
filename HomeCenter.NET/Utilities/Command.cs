using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HomeCenter.NET.Utilities
{
    public class Command : ICloneable
    {
        #region Properties

        public string Data { get; set; }
        public List<string> Keys { get; set; } = new List<string>();
        //public List<string> DeletedKeys { get; set; } TODO: Add

        [JsonIgnore]
        public string KeysString
        {
            get => string.Join(Environment.NewLine, Keys);
            set => Keys = value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        #endregion

        #region Constructors

        public Command()
        {
        }

        public Command(List<string> keys)
        {
            Keys = keys;
        }

        public Command(List<string> keys, string data) : this(keys)
        {
            Data = data;
        }

        public Command(string key) : this(new List<string> { key })
        {
        }

        public Command(string key, string data) : this(key)
        {
            Data = data;
        }

        #endregion

        #region ICloneable

        public object Clone() => new Command(Keys.ToList(), Data);

        #endregion
    }
}
