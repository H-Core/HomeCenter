using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace H.NET.Core.Utilities
{
    public static class StringExtensions
    {
        public static string[] SplitOnlyFirst(this string text, char separator)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (!text.Contains(separator.ToString()))
            {
                return new []{ text, null };
            }

            var array = text.Split(separator);
            var prefix = array.FirstOrDefault();
            var postfix = string.Join(separator.ToString(), array.Skip(1));
            
            return new[] { prefix, postfix };
        }

        public static string[] SplitOnlyFirstIgnoreQuote(this string text, char separator, char quoteSeparator = '\"')
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (!text.Contains(quoteSeparator.ToString()))
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
                pair[0] = pair[0]?.Replace(GetVariableName(j), matches[j]);
                pair[1] = pair[1]?.Replace(GetVariableName(j), matches[j]);
            }

            return pair;
        }

        private static string GetVariableName(int i) => $"$VARIABLE{i}$";

        public static bool IsAny(this string text, StringComparison comparison, params string [] otherStrings)
        {
            foreach (var str in otherStrings)
            {
                if (string.Equals(text, str, comparison))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsAny(this string text, params string[] otherStrings) =>
            text.IsAny(StringComparison.CurrentCulture, otherStrings);

        public static bool IsAnyOrdinalIgnoreCase(this string text, params string[] otherStrings) =>
            text.IsAny(StringComparison.OrdinalIgnoreCase, otherStrings);
    }
}
