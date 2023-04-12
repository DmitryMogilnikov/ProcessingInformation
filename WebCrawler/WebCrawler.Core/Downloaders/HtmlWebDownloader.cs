using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;
using WebCrawler.Core.Models;

namespace WebCrawler.Core.Downloaders
{
    /// <summary>
    /// Загрузчик HTML-содержимого Web-страниц.
    /// </summary>
    public class HtmlWebDownloader : IWebDownloader, IDisposable
    {
        private const string HrefAttributeName = "href";

        private readonly IUrlResolver _urlResolver;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="urlResolver">Сущность, разрешающая URL-адреса основываясь на URL-адресе страницы, с которой они были получены.</param>
        /// <param name="httpClient">Клиент для HTTP-запросов.</param>
        public HtmlWebDownloader(IUrlResolver urlResolver, HttpClient httpClient)
        {
            _urlResolver = urlResolver;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Метод, пытающийся загрузить содержимое Web-страницы по указанному адресу.
        /// </summary>
        /// <param name="url">Адрес Web-страницы, содержимое которой требуется загрузить.</param>
        /// <returns>Содержимое Web-страницы по адресу <paramref name="url"/> или <see langword="null"/>, если его не удалось загрузить.</returns>
        public async Task<IPageContent?> GetPageContentAsync(Uri url)
        {
            if (!url.IsAbsoluteUri)
                throw new ArgumentException("Требуется абсолютный URL-адрес.", nameof(url));

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;
                
                string html = await response.Content.ReadAsStringAsync();
                return await ParseHtmlAsync(html, url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Метод, освобождающий используемые неуправляемые ресурсы.
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private async Task<PageContent?> ParseHtmlAsync(string html, Uri baseUrl)
        {
            IConfiguration config = Configuration.Default;
            using IBrowsingContext context = BrowsingContext.New(config);
            using IDocument htmlDocument = await context.OpenAsync(request => request.Content(html));
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
