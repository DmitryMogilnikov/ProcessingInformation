using AngleSharp.Dom;
using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;
using WebCrawler.Core.Models;
using AngleSharp.Html.Dom;

namespace WebCrawler.Core.Parsers
{
    /// <summary>
    /// Парсер данных в формате HTTP в содержимое страницы.
    /// </summary>
    public class HtmlContentParser : IContentParser
    {
        private const string HrefAttributeName = "href";

        private readonly IUrlResolver _urlResolver;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="urlResolver">Сущность, разрешающая URL-адреса основываясь на URL-адресе страницы, с которой они были получены.</param>
        public HtmlContentParser(IUrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        /// <summary>
        /// Метод, парсящий данные в формате HTTP в содержимое страницы.
        /// </summary>
        /// <param name="data">Данные, которые требуется распарсить.</param>
        /// <param name="baseUrl">Адрес страницы, с которой были получены данные <paramref name="data"/>.</param>
        /// <returns>
        /// Содержимое страницы, полученное из данных <paramref name="data"/>, 
        /// или <see langword="null"/> если содержимое получить не удалось (в т.ч. если данные имели неверный формат).
        /// </returns>
        public async Task<IPageContent?> ParseContentAsync(string data, Uri baseUrl)
        {
            IConfiguration config = Configuration.Default;
            using IBrowsingContext context = BrowsingContext.New(config);
            using IDocument htmlDocument = await context.OpenAsync(request => request.Content(data));
            if (htmlDocument is null)
                return null;

            string textContent = string.Join("", htmlDocument.GetNodes<IText>(predicate: node => node.Parent is not IHtmlScriptElement)
                                                             .Select(node => node.Text));
            IEnumerable<Uri> links = ExtractLinks(htmlDocument, baseUrl).ToArray();

            return new PageContent(textContent, links);
        }

        private IEnumerable<Uri> ExtractLinks(IDocument document, Uri baseUrl)
        {
            IEnumerable<IHtmlAnchorElement> anchors = document.GetNodes<IHtmlAnchorElement>(predicate: anchor => anchor.HasAttribute(HrefAttributeName));
            foreach (IHtmlAnchorElement anchor in anchors)
            {
                string url = anchor.GetAttribute(HrefAttributeName)!;
                if (_urlResolver.TryResolveUrl(url, baseUrl, out Uri? resolvedUrl))
                    yield return resolvedUrl;
            }
        }
    }
}
