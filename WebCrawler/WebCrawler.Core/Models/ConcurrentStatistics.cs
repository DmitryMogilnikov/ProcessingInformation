using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Models
{
    /// <summary>
    /// Класс, хранящий статистику сбора страниц.
    /// </summary>
    /// <remarks>Все методы этого класса потокобезопасны.</remarks>
    public class ConcurrentStatistics : IStatistics
    {
        private ulong _processedPagesCount;
        private ulong _totalLinksCount;
        private ulong _uniqueInternalLinksCount;
        private ulong _brokenLinksCount;
        private readonly ConcurrentDictionary<string, byte> _uniqueInternalSubdomens = new();
        private ulong _totalExternalLinksCount;
        private readonly ConcurrentDictionary<string, byte> _uniqueDocxLinks = new();
        private readonly ConcurrentDictionary<string, byte> _uniqueDocLinks = new();
        private readonly ConcurrentDictionary<string, byte> _uniquePdfLinks = new();

        /// <summary>
        /// Количество обработанных страниц.
        /// </summary>
        public ulong ProcessedPagesCount 
        {
            get => _processedPagesCount;
        }

        /// <summary>
        /// Суммарное количество любых ссылок на обработанных страницах. 
        /// </summary>
        public ulong TotalLinksCount
        { 
            get => _totalLinksCount;
        }

        /// <summary>
        /// Количество уникальных внутренних ссылок.
        /// </summary>
        public ulong UniqueInternalLinksCount
        {
            get => _uniqueInternalLinksCount;
        }

        /// <summary>
        /// Количество ссылок, по которым не получилось получить данные.
        /// </summary>
        public ulong BrokenLinksCount
        {
            get => _brokenLinksCount;
        }

        /// <summary>
        /// Набор уникальных внутренних поддоменов.
        /// </summary>
        public IReadOnlyCollection<string> UniqueInternalSubdomens
        {
            get => (IReadOnlyCollection<string>)_uniqueInternalSubdomens.Keys;
        }

        /// <summary>
        /// Суммарное количество внешних ссылок.
        /// </summary>
        public ulong TotalExternalLinksCount
        {
            get => _totalExternalLinksCount;
        }

        /// <summary>
        /// Набор уникальных ссылок на DOCX-документы.
        /// </summary>
        public IReadOnlyCollection<string> UniqueDocxLinks
        {
            get => (IReadOnlyCollection<string>)_uniqueDocxLinks.Keys;
        }

        /// <summary>
        /// Набор уникальных ссылок на DOC-документы.
        /// </summary>
        public IReadOnlyCollection<string> UniqueDocLinks
        {
            get => (IReadOnlyCollection<string>)_uniqueDocLinks.Keys;
        }

        /// <summary>
        /// Набор уникальных ссылок на PDF-документы.
        /// </summary>
        public IReadOnlyCollection<string> UniquePdfLinks
        {
            get => (IReadOnlyCollection<string>)_uniquePdfLinks.Keys;
        }

        /// <summary>
        /// Метод, увеличивающий количество обработанных страниц (<see cref="ProcessedPagesCount"/>) на 1.
        /// </summary>
        public void IncrementProcessedPagesCount()
        {
            Interlocked.Increment(ref _processedPagesCount);
        }

        /// <summary>
        /// Метод, увеличивающий суммарное количество ссылок на обработанных страницах (<see cref="TotalLinksCount"/>) на 1. 
        /// </summary>
        public void IncrementTotalLinksCount()
        {
            Interlocked.Increment(ref _totalLinksCount);
        }

        /// <summary>
        /// Метод, увеличивающий количество уникальных внутренних ссылок (<see cref="UniqueInternalLinksCount"/>) на указанную величину.
        /// </summary>
        /// <param name="value">Величина, на которую требуется увеличить количество уникальных внутренних ссылок.</param>
        public void AddToUniqueInternalLinksCount(ulong value)
        {
            Interlocked.Add(ref _uniqueInternalLinksCount, value);
        }

        /// <summary>
        /// Метод, увеличивающий количество ссылок, по которым не получилось получить данные (<see cref="BrokenLinksCount"/>), на 1.
        /// </summary>
        public void IncrementBrokenLinksCount()
        {
            Interlocked.Increment(ref _brokenLinksCount);
        }

        /// <summary>
        /// Метод, добавляющий указанный внутренний субдомен в набор <see cref="UniqueInternalSubdomens"/>, если его там ещё нет.
        /// </summary>
        /// <param name="subdomen">Внутренний субдомен, который требуется добавить в <see cref="UniqueInternalSubdomens"/>.</param>
        public void AddInternalSubdomen(string subdomen)
        {
            _ = _uniqueInternalSubdomens.TryAdd(subdomen, 0);
        }

        /// <summary>
        /// Метод, увеличивающий суммарное количество внешних ссылок. (<see cref="TotalExternalLinksCount"/>) на 1.
        /// </summary>
        public void IncrementExternalLinksCount()
        {
            Interlocked.Increment(ref _totalExternalLinksCount);
        }

        /// <summary>
        /// Метод, добавляющий указанную ссылку на DOCX-документ в набор <see cref="UniqueDocxLinks"/>, если её там ещё нет.
        /// </summary>
        /// <param name="subdomen">Ссылка на DOCX-документ, которую требуется добавить в <see cref="UniqueDocxLinks"/>.</param>
        public void AddDocxLink(string link)
        {
            _ = _uniqueDocxLinks.TryAdd(link, 0);
        }

        /// <summary>
        /// Метод, добавляющий указанную ссылку на DOC-документ в набор <see cref="UniqueDocLinks"/>, если её там ещё нет.
        /// </summary>
        /// <param name="subdomen">Ссылка на DOC-документ, которую требуется добавить в <see cref="UniqueDocLinks"/>.</param>
        public void AddDocLink(string link)
        {
            _ = _uniqueDocLinks.TryAdd(link, 0);
        }

        /// <summary>
        /// Метод, добавляющий указанную ссылку на PDF-документ в набор <see cref="UniquePdfLinks"/>, если её там ещё нет.
        /// </summary>
        /// <param name="subdomen">Ссылка на PDF-документ, которую требуется добавить в <see cref="UniquePdfLinks"/>.</param>
        public void AddPdfLink(string link)
        {
            _ = _uniquePdfLinks.TryAdd(link, 0);
        }

        /// <summary>
        /// Метод, возвращающий строковое представление данного экземпляра класса <see cref="ConcurrentStatistics"/>.
        /// </summary>
        public override string ToString()
        {
            return
                $"""
                 Всего собрано страниц: {ProcessedPagesCount}.
                 Всего ссылок на страницах: {TotalLinksCount}.
                 Уникальных внутренних ссылок: {UniqueInternalLinksCount}.
                 Ссылок, по которым не удалось получить страницы: {BrokenLinksCount}.
                 Уникальных внутренних поддоменов: {UniqueInternalSubdomens.Count}.
                 Всего внешних ссылок: {TotalExternalLinksCount}.
                 Уникальных ссылок на DOCX-документы: {UniqueDocxLinks.Count}.
                 Уникальных ссылок на DOC-документы: {UniqueDocLinks.Count}.
                 Уникальных ссылок на PDF-документы: {UniquePdfLinks.Count}.
                 """;
        }
    }
}
