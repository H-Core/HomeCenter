using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H.NET.Core;
using HtmlAgilityPack;

namespace H.NET.Searchers
{
    public class YandexSearcher : Module, ISearcher
    {
        public async Task<List<string>> Search(string query)
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
