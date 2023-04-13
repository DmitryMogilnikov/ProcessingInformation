using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Parsers;

namespace WebCrawler.Core.Factories
{
    /// <summary>
    /// Фабрика парсеров данных в формате HTTP в содержимое страницы.
    /// </summary>
    public class HtmlContentParserFactory : IFactory<IContentParser>
    {
        private readonly IUrlResolver _urlResolver;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="urlResolver">Сущность, разрешающая URL-адреса основываясь на URL-адресе страницы, с которой они были получены.</param>
        public HtmlContentParserFactory(IUrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        /// <summary>
        /// Метод, возвращающий новый экземпляр парсера данных в формате HTTP в содержимое страницы.
        /// </summary>
        public IContentParser Create()
        {
            return new HtmlContentParser(_urlResolver);
        }
    }
}
