using System;
using System.Collections.Generic;
using System.Linq;

namespace H.NET.Runners.Searchers
{
    public static class SearchManager
    {
        public static List<string> Go<T>(string query) where T : ISearcher, new()
        {
            try
            {
                var searcher = new T();

                return searcher.Search(query);
            }
            catch (Exception exception)
            {
                //Log(exception.ToString());
                return new List<string> { exception.ToString() };
            }
        }

        public static List<string> Go<T>(string query, int count) where T : ISearcher, new() =>
            Go<T>(query).Take(count).ToList();
    }
}
