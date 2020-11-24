using System.Collections.Generic;
using System.Threading.Tasks;

namespace H.NET.Core
{
    public interface ISearcher : IModule
    {
        Task<List<string>> Search(string query);
    }
}
