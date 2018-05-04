using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace H.NET.Runners
{
    public static class GoogleSearch
    {
        /*
        private List<string> GoogleCommand(string query)
        {
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
        }*/
        
        public static List<string> Go(string query)
        {
            try
            {
                var url = $"https://www.yandex.ru/search/?text={query}";

                var web = new HtmlWeb();
                var document = web.Load(url);

                return document.DocumentNode
                    .SelectNodes("//a[@href]")
                    .Where(i => i.Attributes.Contains("tabindex") && i.Attributes["tabindex"].Value == "2")
                    .Select(i => i.Attributes["href"].Value)
                    .ToList();
            }
            catch (Exception exception)
            {
                //Log(exception.ToString());
                return new List<string> { exception.ToString() };
            }
        }
    }
}
