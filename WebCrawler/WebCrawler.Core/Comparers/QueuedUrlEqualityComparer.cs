using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Comparers
{
    /// <summary>
    /// Компаратор для сравнения на равенство URL-адресов в очереди, учитывающий только непосредственно адреса. 
    /// </summary>
    public class QueuedUrlEqualityComparer : IEqualityComparer<IQueuedUrl>
    {
        private static QueuedUrlEqualityComparer? _instance;

        /// <summary>
        /// Экземпляр компаратора.
        /// </summary>
        public static QueuedUrlEqualityComparer Instance
        {
            get
            {
                _instance ??= new QueuedUrlEqualityComparer();
                return _instance;
            }
        }

        private QueuedUrlEqualityComparer()
        {
        }

        /// <summary>
        /// Метод, сравнивающий два URL-адреса в очереди на равенство.
        /// </summary>
        /// <param name="x">Первый URL-адрес в очереди.</param>
        /// <param name="y">Второй URL-адрес в очереди.</param>
        /// <returns>
        /// Если <paramref name="x"/> и <paramref name="y"/> эквивалентны (в т.ч. если они оба <see langword="null"/>) -
        /// <see langword="true"/>, иначе - <see langword="false"/>.
        /// </returns>
        public bool Equals(IQueuedUrl? x, IQueuedUrl? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;
            return x.Url.Equals(y.Url);
        }

        /// <summary>
        /// Метод, возвращающий хеш-код URL-адреса в очереди.
        /// </summary>
        /// <param name="obj">URL-адрес в очереди, хеш-код которого требуется получить.</param>
        /// <returns>Хеш-код <paramref name="obj"/>.</returns>
        public int GetHashCode([DisallowNull] IQueuedUrl obj)
        {
            return obj.Url.GetHashCode();
        }
    }
}
