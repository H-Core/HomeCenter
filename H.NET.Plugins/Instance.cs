using System;
using System.Linq;

namespace H.NET.Plugins
{
    public class Instance
    {
        #region Static methods

        public static Instance FromString(string text)
        {
            var values = text.Split(Separator);

            return new Instance
            {
                Name = values.ElementAtOrDefault(0),
                TypeName = values.ElementAtOrDefault(1),
                IsEnabled = bool.TryParse(values.ElementAtOrDefault(2), out var result) && result
            };
        }

        #endregion

        #region Properties

        public const char Separator = ' ';

        public string Name { get; set; }
        public string TypeName { get; set; }
        public bool IsEnabled { get; set; }

        #endregion

        #region Public methods

        public override string ToString() => $"{Name}{Separator}{TypeName}{Separator}{IsEnabled}";

        #endregion

    }
}