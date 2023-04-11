using System;
using System.Text.Json.Serialization;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Models
{
    /// <summary>
    /// Рекорд, описывающий URL-адрес в очереди.
    /// </summary>
    public record QueuedUrl : IQueuedUrl
    {
        /// <summary>
        /// URL-адрес.
        /// </summary>
        public Uri Url { get; }

        /// <summary>
        /// Количество уже сделанных попыток получить содержимое Web-страницы по адресу <see cref="RequestsCount"/>.
        /// </summary>
        public int RequestsCount { get; }

        /// <summary>
        /// Конструктор для URL-адресов, которые ещё ни разу не запрашивались.
        /// </summary>
        /// <param name="url">URL-адрес.</param>
        public QueuedUrl(Uri url) : this(url, 0)
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="url">URL-адрес.</param>
        /// <param name="requestsCount">Количество уже сделанных попыток получить содержимое Web-страницы по адресу <see cref="RequestsCount"/>.</param>
        [JsonConstructor]
        public QueuedUrl(Uri url, int requestsCount)
        {
            Url = url;
            RequestsCount = requestsCount; 
        }
    }
}
