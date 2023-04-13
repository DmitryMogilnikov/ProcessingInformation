using System;
using System.Threading.Tasks;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Interfaces
{
    /// <summary>
    /// Интерфайс парсера данных в содержимое страницы.
    /// </summary>
    public interface IContentParser
    {
        /// <summary>
        /// Метод, парсящий данные в содержимое страницы.
        /// </summary>
        /// <param name="data">Данные, которые требуется распарсить.</param>
        /// <param name="baseUrl">Адрес страницы, с которой были получены данные <paramref name="data"/>.</param>
        /// <returns>
        /// Содержимое страницы, полученное из данных <paramref name="data"/>, 
        /// или <see langword="null"/> если содержимое получить не удалось.
        /// </returns>
        Task<IPageContent?> ParseContentAsync(string data, Uri baseUrl);
    }
}
