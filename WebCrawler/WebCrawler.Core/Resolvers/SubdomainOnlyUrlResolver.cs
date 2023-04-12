using Nager.PublicSuffix;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using WebCrawler.Core.Interfaces;

namespace WebCrawler.Core.Resolvers
{
    /// <summary>
    /// Класс, разрешающий URL-адреса основываясь на базовом URL-адресе и проверяющий, 
    /// что получившийся URL имеет тот же домен, что и базовый, отличаясь, возможно, лишь поддоменами.
    /// </summary>
    public class SubdomainOnlyUrlResolver : UrlResolver, IUrlResolver
    {
        /// <summary>
        /// Метод, пытающийся разрешить URL-адрес. 
        /// Этот метод проверяет, что получившийся URL имеет тот же домен, что и <paramref name="baseUrl"/>, 
        /// отличаясь, возможно, лишь поддоменами.
        /// </summary>
        /// <param name="url">URL-адрес, который требуется разрешить.</param>
        /// <param name="baseUrl">Базовый URL-адрес.</param>
        /// <param name="resolvedUrl">Разрешённый URL-адрес или <see langword="null"/>, если адрес разрешить не удалось.</param>
        /// <returns>
        /// Если URL-адрес удалось разрешить и его домен совпадает с доменом <paramref name="baseUrl"/> - <see langword="true"/>,
        /// иначе - <see langword="false"/>.
        /// </returns>
        /// <remarks>Если <paramref name="url"/> - абсолютный URL, то вернётся он же, <paramref name="baseUrl"/> будет проигнорирован.</remarks>
        public override bool TryResolveUrl(string url, Uri baseUrl, [NotNullWhen(true)] out Uri? resolvedUrl)
        {
            resolvedUrl = null;

            if (!baseUrl.IsAbsoluteUri)
                return false;

            try
            {
                Uri combinedUrl = CombineWithBase(url, baseUrl);

                DomainParser domainParser = new(new WebTldRuleProvider());
                if (domainParser.Parse(combinedUrl).RegistrableDomain != domainParser.Parse(baseUrl).RegistrableDomain)
                    return false;

                resolvedUrl = combinedUrl;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}
