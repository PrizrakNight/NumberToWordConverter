using HtmlAgilityPack;
using NumberToWordConverter.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NumberToWordConverter
{
    public class SimpleSiteService
    {
        private readonly string _siteBase;

        private readonly HttpClient _httpClient;

        public SimpleSiteService(string siteBase)
        {
            _siteBase = siteBase;
            _httpClient = new HttpClient();
        }

        public Task<ReadOnlyCollection<LanguageItem>> GetLanguagesAsync() => Task.Factory.StartNew(() =>
        {
            var web = new HtmlWeb();
            var document = web.Load(_siteBase);
            var languageSelection = document.DocumentNode.SelectSingleNode("//select[@name='lang']");
            var languageItems = new List<LanguageItem>();

            foreach (var node in languageSelection.ChildNodes.Where(node => node.NodeType != HtmlNodeType.Text))
            {
                var languageKey = node.Attributes["value"].Value;
                var languageName = node.InnerText;

                languageName = FirstLetterToUppercase(languageName);

                languageItems.Add(new LanguageItem
                {
                    Key = languageKey,
                    Name = languageName
                });
            }

            return new ReadOnlyCollection<LanguageItem>(languageItems.OrderBy(item => item.Name).ToList());
        });

        public async Task<string> ConvertToWordsAsync(int value, LanguageItem language)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "number", value.ToString()},
                {"lang", language.Key }
            };

            var content = new FormUrlEncodedContent(queryParams);
            var httpResponse = await _httpClient.PostAsync(_siteBase, content);
            var htmlResult = await httpResponse.Content.ReadAsStringAsync();
            var document = new HtmlDocument();

            document.LoadHtml(htmlResult);

            var resultString = document.DocumentNode.SelectSingleNode("//p/span[@class='inwords']/text()");

            return FirstLetterToUppercase(resultString.InnerText);
        }

        private string FirstLetterToUppercase(string input) => char.ToUpper(input[0]) + input.Substring(1);
    }
}
