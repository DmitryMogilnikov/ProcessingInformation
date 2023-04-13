using System;
using System.Threading.Tasks;

namespace WebCrawler.Core.Interfaces
{
    /// <summary>
    /// Интерфейс получателя данных из источников конкретного вида.
    /// </summary>
    public interface IDataGetter
    {
        /// <summary>
        /// Метод, проверяющий, что этот получатель данных может получить данные из источника, задаваемого указанным URL-адресом.
        /// </summary>
        /// <param name="url">URL-адрес источника, возможность получения данных из которого требуется проверить.</param>
        /// <returns>
        /// Если этот получатель может получить данные из источника, задаваемого <paramref name="url"/> -
        /// <see langword="true"/>, иначе - <see langword="false"/>.
        /// </returns>
        bool CanGetContent(Uri url);

        /// <summary>
        /// Метод, получающий данные из источника.
        /// </summary>
        /// <param name="url">URL-адрес источника, из которого требуется получить данные.</param>
        /// <returns>
        /// Данные, полученные из источника с адресом <paramref name="url"/>, 
        /// или <see langword="null"/>, если адрес источника корректный, но данные по какой-то причине получить не удалось.
        /// </returns>
        Task<string?> GetContentAsync(Uri url);
    }
}
