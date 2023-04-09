using System;

namespace WebCrawler.Core.Interfaces.Models
{
    /// <summary>
    /// Интерфейс URL-адреса в очереди.
    /// </summary>
    public interface IQueuedUrl
    {
        /// <summary>
        /// URL-адрес.
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// Количество уже сделанных попыток получить содержимое Web-страницы по адресу <see cref="RequestsCount"/>.
        /// </summary>
        int RequestsCount { get; }
    }
}
