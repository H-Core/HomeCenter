using System;
using System.Linq;

namespace VoiceActions.NET.Utilities
{
    public static class StringExtensions
    {
        public static (string, string) SplitOnlyFirst(this string text, char separator)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (!text.Contains(separator))
            {
                return (text, null);
            }

            var array = text.Split(separator);
            var prefix = array.FirstOrDefault();
            var postfix = string.Join(separator.ToString(), array.Skip(1));
            
            return (prefix, postfix);
        }
    }
}
