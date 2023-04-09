using System;
using System.Diagnostics.CodeAnalysis;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Interfaces
{
    /// <summary>
    /// Интерфейс загрузчика содержимого Web-страниц.
    /// </summary>
    public interface IWebDownloader
    {
        /// <summary>
        /// Метод, пытающийся загрузить содержимое Web-страницы по указанному адресу.
        /// </summary>
        /// <param name="url">Адрес Web-страницы, содержимое которой требуется загрузить.</param>
        /// <param name="pageContent">
        /// Содержимое Web-страницы по адресу <paramref name="url"/> или <see langword="null"/>, если его не удалось загрузить.
        /// </param>
        /// <returns>Если удалось загрузить содержимое Web-страницы - <see langword="true"/>, иначе - <see langword="false"/>.</returns>
        bool TryGetPageContent(Uri url, [NotNullWhen(true)] out IPageContent pageContent);
    }
}
