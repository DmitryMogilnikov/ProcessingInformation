using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using WebCrawler.Core.Interfaces;

namespace WebCrawler.Core.Getters
{
    /// <summary>
    /// Получатель данных из источников, поддерживающих протокол HTTP(S).
    /// </summary>
    public class HttpDataGetter : IDataGetter
    {
        private static readonly HashSet<string> SupportedSchemes = new() { "http", "https" };

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="httpClient">Клиент для HTTP-запросов к источникам.</param>
        public HttpDataGetter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Метод, проверяющий, что источник, задаваемый URL-адресом, поддерживает протокол HTTP(S).
        /// </summary>
        /// <param name="url">URL-адрес источника, который требуется проверить.</param>
        /// <returns>
        /// Если источник, задаваемый адресом <paramref name="url"/>, поддерживает протокол HTTP(S) -
        /// <see langword="true"/>, иначе - <see langword="false"/>.
        /// </returns>
        public bool CanGetContent(Uri url)
        {
            return SupportedSchemes.Contains(url.Scheme);
        }

        /// <summary>
        /// Метод, получающий данные из источника.
        /// </summary>
        /// <param name="url">URL-адрес источника, из которого требуется получить данные.</param>
        /// <returns>
        /// Данные, полученные из источника с адресом <paramref name="url"/>, 
        /// или <see langword="null"/>, если адрес источника корректный, но данные по какой-то причине получить не удалось.
        /// </returns>
        public async Task<string?> GetContentAsync(Uri url)
        {
            if (!url.IsAbsoluteUri)
                throw new ArgumentException("Требуется абсолютный URL-адрес.", nameof(url));

            if (!CanGetContent(url))
                throw new NotSupportedException($"Неподдерживаемая схема: {url.Scheme}.");

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"StatusCode: {response.StatusCode}, Reason: {response.ReasonPhrase}, URL: {url}");
                    return null;
                }

                return await response.Content.ReadAsStringAsync();;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
