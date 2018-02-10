using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VoiceActions.NET.Utilities
{
    public static class StringExtensions
    {
        public static (string prefix, string postfix) SplitOnlyFirst(this string text, char separator)
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

        public static (string prefix, string postfix) SplitOnlyFirstIgnoreQuote(this string text, char separator, char quoteSeparator = '\"')
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (!text.Contains(quoteSeparator))
            {
                return text.SplitOnlyFirst(separator);
            }

            var matches = new List<string>();
            var i = 0;
            var replacedText = Regex.Replace(text, $"{quoteSeparator}(.*?){quoteSeparator}", match =>
            {
                matches.Add(match.ToString());
                var name = GetVariableName(i);
                i++;

                return name;
            });

            var pair = replacedText.SplitOnlyFirst(separator);
            for (var j = 0; j < i; j++)
            {
                pair.prefix = pair.prefix?.Replace(GetVariableName(j), matches[j]);
                pair.postfix = pair.postfix?.Replace(GetVariableName(j), matches[j]);
            }

            return pair;
        }

        private static string GetVariableName(int i) => $"$VARIABLE{i}$";
    }
}
