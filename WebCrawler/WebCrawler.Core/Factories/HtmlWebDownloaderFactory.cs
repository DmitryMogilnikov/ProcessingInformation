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
