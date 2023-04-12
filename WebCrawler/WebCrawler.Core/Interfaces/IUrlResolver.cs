using System;
using System.Diagnostics.CodeAnalysis;

namespace WebCrawler.Core.Interfaces
{
    /// <summary>
    /// Интерфейс сущности, разрешающей URL-адреса основываясь на базовом URL-адресе.
    /// </summary>
    public interface IUrlResolver
    {
        /// <summary>
        /// Метод, пытающийся разрешить URL-адрес.
        /// </summary>
        /// <param name="url">URL-адрес, который требуется разрешить.</param>
        /// <param name="baseUrl">Базовый URL-адрес.</param>
        /// <param name="resolvedUrl">Разрешённый URL-адрес или <see langword="null"/>, если адрес разрешить не удалось.</param>
        /// <returns>Если URL-адрес удалось разрешить - <see langword="true"/>, иначе - <see langword="false"/>.</returns>
        bool TryResolveUrl(string url, Uri baseUrl, [NotNullWhen(true)] out Uri? resolvedUrl);
    }
}
