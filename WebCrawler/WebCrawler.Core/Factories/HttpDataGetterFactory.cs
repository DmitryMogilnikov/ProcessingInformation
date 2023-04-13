using System;
using System.Net.Http;
using WebCrawler.Core.Getters;
using WebCrawler.Core.Interfaces;

namespace WebCrawler.Core.Factories
{
    /// <summary>
    /// Фабрика получателей данных из источников, поддерживающих протокол HTTP(S).
    /// </summary>
    public class HttpDataGetterFactory : IFactory<IDataGetter>, IDisposable
    {
        private const string UserAgentHeagerHame = "User-Agent";
        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 YaBrowser/23.1.5.710 Yowser/2.5 Safari/537.36";

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public HttpDataGetterFactory()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add(UserAgentHeagerHame, UserAgent);
        }

        /// <summary>
        /// Метод, возвращающий новый экземпляр получателя данных из источников, поддерживающих протокол HTTP(S).
        /// </summary>
        public IDataGetter Create()
        {
            return new HttpDataGetter(_httpClient);
        }

        /// <summary>
        /// Метод, освобождающий используемые неуправляемые ресурсы.
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
