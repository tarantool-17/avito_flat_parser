using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AvitoFlats
{
    public class AvitoFlatParser
    {
        public async Task<IEnumerable<Flat>> ParseDocument(HtmlDocument document)
        {
            return await Task.Run(() =>
            {
                var items = new List<Flat>();
                var nodes = document.DocumentNode.SelectNodes("//div[contains(@class,'item_table-wrapper')]");
                foreach (var flat in nodes)
                {
                    items.Add(new Flat
                    {
                        Price = flat.SelectSingleNode(".//span[contains(@class, 'price')]").Attributes["content"]?.Value,
                        Header = flat.SelectSingleNode(".//h3[contains(@class, 'item-description-title')]")?.InnerText,
                        Commission = flat.SelectSingleNode(".//span[contains(@class, 'about__commission')]")?.InnerText,
                        Address = flat.SelectSingleNode(".//p[contains(@class, 'address')]")?.InnerText,
                        Owner = flat.SelectSingleNode(".//div[contains(@class, 'data')]")?.InnerText,
                        Url = flat.SelectSingleNode(".//a[contains(@class, 'item-description-title-link')]").Attributes["href"]?.Value,
                        MetroDistanceStr = flat.SelectSingleNode(".//span[contains(@class, 'c-2')]")?.InnerText
                    });
                }
                return items;
            });
        }
    }
}