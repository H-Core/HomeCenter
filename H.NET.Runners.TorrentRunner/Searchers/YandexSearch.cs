using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace H.NET.Runners.Searchers
{
    public class YandexSearcher : ISearcher
    {
        public List<string> Search(string query)
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
    }
}
