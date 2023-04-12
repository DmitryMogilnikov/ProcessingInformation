using System;
using System.Threading.Tasks;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Interfaces
{
    /// <summary>
    /// Интерфейс загрузчика содержимого Web-страниц.
    /// </summary>
    public interface IWebDownloader : IDisposable
    {
        /// <summary>
        /// Метод, пытающийся загрузить содержимое Web-страницы по указанному адресу.
        /// </summary>
        /// <param name="url">Адрес Web-страницы, содержимое которой требуется загрузить.</param>
        /// <returns>Содержимое Web-страницы по адресу <paramref name="url"/> или <see langword="null"/>, если его не удалось загрузить.</returns>
        Task<IPageContent?> GetPageContentAsync(Uri url);
    }
}
