using System;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Models
{
    /// <summary>
    /// Рекорд, описывающий URL-адрес в очереди.
    /// </summary>
    /// <param name="Url">URL-адрес.</param>
    /// <param name="RequestsCount">Количество уже сделанных попыток получить содержимое Web-страницы по адресу <see cref="RequestsCount"/>.</param>
    public record QueuedUrl(Uri Url, int RequestsCount) : IQueuedUrl
    {
        /// <summary>
        /// Конструктор для URL-адресов, которые ещё ни разу не запрашивались.
        /// </summary>
        /// <param name="url">URL-адрес.</param>
        public QueuedUrl(Uri url) : this(url, 0)
        {
        }
    }
}
