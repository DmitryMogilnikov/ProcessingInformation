using System.Collections.Generic;

namespace WebCrawler.Core.Interfaces.Models
{
    /// <summary>
    /// Интерфейс сущности, хранящей статистику сбора страниц.
    /// </summary>
    public interface IStatistics
    {
        /// <summary>
        /// Количество обработанных страниц.
        /// </summary>
        ulong ProcessedPagesCount { get; }

        /// <summary>
        /// Суммарное количество любых ссылок на обработанных страницах. 
        /// </summary>
        ulong TotalLinksCount { get; }

        /// <summary>
        /// Количество уникальных внутренних ссылок.
        /// </summary>
        ulong UniqueInternalLinksCount { get; }

        /// <summary>
        /// Количество ссылок, по которым не получилось получить данные.
        /// </summary>
        ulong BrokenLinksCount { get; }

        /// <summary>
        /// Набор уникальных внутренних поддоменов.
        /// </summary>
        IReadOnlyCollection<string> UniqueInternalSubdomens { get; }

        /// <summary>
        /// Суммарное количество внешних ссылок.
        /// </summary>
        ulong TotalExternalLinksCount { get; }

        /// <summary>
        /// Набор уникальных ссылок на DOCX-документы.
        /// </summary>
        IReadOnlyCollection<string> UniqueDocxLinks { get; }

        /// <summary>
        /// Набор уникальных ссылок на DOC-документы.
        /// </summary>
        IReadOnlyCollection<string> UniqueDocLinks { get; }

        /// <summary>
        /// Набор уникальных ссылок на PDF-документы.
        /// </summary>
        IReadOnlyCollection<string> UniquePdfLinks { get; }

        /// <summary>
        /// Метод, увеличивающий количество обработанных страниц (<see cref="ProcessedPagesCount"/>) на 1.
        /// </summary>
        void IncrementProcessedPagesCount();

        /// <summary>
        /// Метод, увеличивающий суммарное количество ссылок на обработанных страницах (<see cref="TotalLinksCount"/>) на 1. 
        /// </summary>
        void IncrementTotalLinksCount();

        /// <summary>
        /// Метод, увеличивающий количество уникальных внутренних ссылок (<see cref="BrokenLinksCount"/>) на указанную величину.
        /// </summary>
        /// <param name="value">Величина, на которую требуется увеличить количество уникальных внутренних ссылок.</param>
        void AddToUniqueInternalLinksCount(ulong value);

        /// <summary>
        /// Метод, увеличивающий количество ссылок, по которым не получилось получить данные (<see cref="BrokenLinksCount"/>), на 1.
        /// </summary>
        void IncrementBrokenLinksCount();

        /// <summary>
        /// Метод, добавляющий указанный внутренний субдомен в набор <see cref="UniqueInternalSubdomens"/>, если его там ещё нет.
        /// </summary>
        /// <param name="subdomen">Внутренний субдомен, который требуется добавить в <see cref="UniqueInternalSubdomens"/>.</param>
        void AddInternalSubdomen(string subdomen);

        /// <summary>
        /// Метод, увеличивающий суммарное количество внешних ссылок. (<see cref="TotalExternalLinksCount"/>) на 1.
        /// </summary>
        void IncrementExternalLinksCount();

        /// <summary>
        /// Метод, добавляющий указанную ссылку на DOCX-документ в набор <see cref="UniqueDocxLinks"/>, если её там ещё нет.
        /// </summary>
        /// <param name="subdomen">Ссылка на DOCX-документ, которую требуется добавить в <see cref="UniqueDocxLinks"/>.</param>
        void AddDocxLink(string link);

        /// <summary>
        /// Метод, добавляющий указанную ссылку на DOC-документ в набор <see cref="UniqueDocLinks"/>, если её там ещё нет.
        /// </summary>
        /// <param name="subdomen">Ссылка на DOC-документ, которую требуется добавить в <see cref="UniqueDocLinks"/>.</param>
        void AddDocLink(string link);

        /// <summary>
        /// Метод, добавляющий указанную ссылку на PDF-документ в набор <see cref="UniquePdfLinks"/>, если её там ещё нет.
        /// </summary>
        /// <param name="subdomen">Ссылка на PDF-документ, которую требуется добавить в <see cref="UniquePdfLinks"/>.</param>
        void AddPdfLink(string link);
    }
}
