using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using WebCrawler.Core.Interfaces;

namespace WebCrawler.Core.Resolvers
{
    /// <summary>
    /// Класс, разрешающий URL-адреса основываясь на базовом URL-адресе.
    /// </summary>
    public class UrlResolver : IUrlResolver
    {
        /// <summary>
        /// Метод, пытающийся разрешить URL-адрес.
        /// </summary>
        /// <param name="url">URL-адрес, который требуется разрешить.</param>
        /// <param name="baseUrl">Базовый URL-адрес.</param>
        /// <param name="resolvedUrl">Разрешённый URL-адрес или <see langword="null"/>, если адрес разрешить не удалось.</param>
        /// <returns>Если URL-адрес удалось разрешить - <see langword="true"/>, иначе - <see langword="false"/>.</returns>
        /// <remarks>Если <paramref name="url"/> - абсолютный URL, то вернётся он же, <paramref name="baseUrl"/> будет проигнорирован.</remarks>
        public virtual bool TryResolveUrl(string url, Uri baseUrl, [NotNullWhen(true)] out Uri? resolvedUrl)
        {
            resolvedUrl = null;

            if (!baseUrl.IsAbsoluteUri)
                return false;

            try
            {
                resolvedUrl = CombineWithBase(url, baseUrl);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false; 
            }
        }

        /// <summary>
        /// Метод, объединяющий URL-адрес с базовым с учётом особенностей объединения объектов <see cref="Uri"/>.
        /// </summary>
        /// <param name="url">URL-адрес, который требуется объединить с базовым.</param>
        /// <param name="baseUrl">Базовый URL-адрес.</param>
        /// <returns>URL-адрес, полученный объединением <paramref name="url"/> с <paramref name="baseUrl"/>.</returns>
        /// <remarks>
        /// Если <paramref name="url"/> - абсолютный URL, то вернётся он же, <paramref name="baseUrl"/> будет проигнорирован.<br/>
        /// По какой-то неведомой причине объединение посредством конструктора класса <see cref="Uri"/> 
        /// правильно работает только если в конце <paramref name="baseUrl"/> есть слеш. Этот метод <paramref name="baseUrl"/> к нужному виду,
        /// а заодно добавляет в слеш конец результата, чтобы избежать проблем, если этот результат тоже с чем-то понадобится объединить.
        /// </remarks>
        protected static Uri CombineWithBase(string url, Uri baseUrl)
        {
            string baseUrlString = baseUrl.IsFile ? (Path.GetDirectoryName(baseUrl.AbsolutePath.TrimEnd('/')) ?? "file:///") : baseUrl.ToString();

            if (!baseUrlString.EndsWith("/"))
            {
                baseUrlString += "/";
                baseUrl = new Uri(baseUrlString);
            }

            if (!url.EndsWith("/"))
                url += "/";

            return new Uri(baseUrl, url);
        }
    }
}
