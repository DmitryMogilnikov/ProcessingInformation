using System;
using System.IO;
using System.Threading.Tasks;
using WebCrawler.Core.Interfaces;

namespace WebCrawler.Core.Getters
{
    /// <summary>
    /// Получатель данных из источников-файлов.
    /// </summary>
    public class FileDataGetter : IDataGetter
    {
        /// <summary>
        /// Метод, проверяющий, что источник, задаваемый URL-адресом, является файловым.
        /// </summary>
        /// <param name="url">URL-адрес источника, который требуется проверить.</param>
        /// <returns>
        /// Если источник, задаваемый адресом <paramref name="url"/>, является файловым -
        /// <see langword="true"/>, иначе - <see langword="false"/>.
        /// </returns>
        public bool CanGetContent(Uri url)
        {
            return url.IsFile;
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
            string path = url.LocalPath.TrimEnd('\\');

            if (!File.Exists(path))
                return null;

            return await File.ReadAllTextAsync(path);
        }
    }
}
