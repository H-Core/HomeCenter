using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Storages.Extensions
{
    public static class EnumerableExtensions
    {
        public static List<T> UniqueValues<T>(this IEnumerable<T> enumerable, Func<T, object> func) => enumerable
            ?.GroupBy(func)
            .Select(group => group.First())
            .ToList();
    }
}
