using System.Collections.Generic;
using System.Threading.Tasks;
using H.Core;

namespace H.Searchers
{
    public class GoogleSearcher : Module, ISearcher
    {
        public async Task<List<string>> Search(string query)
        {
            await Task.Delay(0);
            /*
            using (var service = new CustomsearchService(
                new BaseClientService.Initializer
                {
                    ApiKey = GoogleSearchApiKey
                }))
            {
                var requests = service.Cse.List(query);
                requests.Cx = GoogleCx;
                requests.Num = MaxResults;

                var results = requests.Execute().Items;
                if (results == null)
                {
                    return new List<string>();
                }

                return results.Select(i => i.Link).ToList();
            }
            */
            return new List<string>();
        }
    }
}
