using System.Collections.Generic;

namespace H.NET.Runners.Searchers
{
    public class GoogleSearcher : ISearcher
    {
        public List<string> Search(string query)
        {
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
