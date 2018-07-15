using System.Collections.Generic;

namespace H.NET.Runners.Searchers
{
    public interface ISearcher
    {
        List<string> Search(string query);
    }
}
