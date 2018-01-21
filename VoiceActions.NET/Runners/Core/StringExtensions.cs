using System;
using System.Linq;

namespace VoiceActions.NET.Runners.Core
{
    public static class StringExtensions
    {
        public static (string, string) SplitOnlyFirst(this string text, char separator)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            var prefix = text.Split(separator).FirstOrDefault();
            var postfix = text.Replace(prefix ?? "", string.Empty).Trim(separator);

            return (prefix, postfix);
        }
    }
}
