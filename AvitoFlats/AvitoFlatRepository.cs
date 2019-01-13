using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AvitoFlats
{
    public class AvitoFlatRepository
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly AvitoFlatParser _parser = new AvitoFlatParser();
        private readonly string _url;

        public AvitoFlatRepository(string url)
        {
            _url = url;
        }
        
        public async Task<IEnumerable<Flat>> GetCurrentAsync()
        {
            var items = new List<Flat>();
            
            int page = 1;
            while (true)
            {
                var response = await _client.GetAsync($"{_url}&p={page}");
                if (!response.IsSuccessStatusCode)
                    break;

                var content = await response.Content.ReadAsStringAsync();
                var document = new HtmlDocument();
                document.LoadHtml(content);
                items.AddRange(await _parser.ParseDocument(document));
                
                page++;
            }

            return items;
        }
    }
}