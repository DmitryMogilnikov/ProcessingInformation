using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;
using WebCrawler.Core.Parsers;

namespace WebCrawler.Core.Factories
{
    /// <summary>
    /// Фабрика парсеров данных в формате HTTP в содержимое страницы.
    /// </summary>
    public class HtmlContentParserFactory : IFactory<IContentParser>
    {
        private readonly IUrlResolver _urlResolver;
        private readonly IStatistics _statistics;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="urlResolver">Сущность, разрешающая URL-адреса основываясь на URL-адресе страницы, с которой они были получены.</param>
        /// <param name="statistics">Сущность, хранящая статистику сбора страниц.</param>
        public HtmlContentParserFactory(IUrlResolver urlResolver, IStatistics statistics)
        {
            _urlResolver = urlResolver;
            _statistics = statistics;
        }

        /// <summary>
        /// Метод, возвращающий новый экземпляр парсера данных в формате HTTP в содержимое страницы.
        /// </summary>
        public IContentParser Create()
        {
            return new HtmlContentParser(_urlResolver, _statistics);
        }
    }
}
