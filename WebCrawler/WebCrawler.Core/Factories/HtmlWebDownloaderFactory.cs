using System.Net.Http;
using WebCrawler.Core.Downloaders;
using WebCrawler.Core.Interfaces;

namespace WebCrawler.Core.Factories
{
    /// <summary>
    /// Фабрика загрузчиков HTML-содержимого Web-страниц.
    /// </summary>
    public class HtmlWebDownloaderFactory : IFactory<IWebDownloader>
    {
        private readonly IUrlResolver _urlResolver;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="urlResolver">Сущность, разрешающая URL-адреса основываясь на URL-адресе страницы, с которой они были получены.</param>
        public HtmlWebDownloaderFactory(IUrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 YaBrowser/23.1.5.710 Yowser/2.5 Safari/537.36");
        }

        /// <summary>
        /// Метод, возвращающий новый экземпляр загрузчика HTML-содержимого Web-страниц.
        /// </summary>
        public IWebDownloader Create()
        {
            return new HtmlWebDownloader(_urlResolver, _httpClient);
        }
    }
}
