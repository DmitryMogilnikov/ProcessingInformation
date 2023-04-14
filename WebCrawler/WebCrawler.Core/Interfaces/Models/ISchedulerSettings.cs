using System;
using System.Collections.Generic;

namespace WebCrawler.Core.Interfaces.Models
{
    /// <summary>
    /// Интерфейс настроек планировщика запросов.
    /// </summary>
    public interface ISchedulerSettings
    {
        /// <summary>
        /// Количество загрузчиков, работающих одновременно.
        /// </summary>
        int NumberOfWorkers { get; }

        /// <summary>
        /// Максимальное суммарное количество запросов в секунду.
        /// </summary>
        int RequestsPerSecond { get; }

        /// <summary>
        /// Максимальное количество повторных попыток запросить страницу.
        /// </summary>
        int MaxRetries { get; }

        /// <summary>
        /// Частота сохранения снапшота очереди.
        /// </summary>
        TimeSpan QueueSnapshotSavePeriod { get; }

        /// <summary>
        /// Упорядоченный в порядке уменьшения приоритета набор фабрик получателей данных.
        /// </summary>
        IEnumerable<IFactory<IDataGetter>> DataGetterFactories { get; }

        /// <summary>
        /// Упорядоченный в порядке уменьшения приоритета набор фабрик парсеров данных в содержимое страницы.
        /// </summary>
        IEnumerable<IFactory<IContentParser>> ContentParserFactories { get; }

        /// <summary>
        /// Фабрика сущностей, сохраняющих содержимое Web-страниц.
        /// </summary>
        IFactory<IContentSaver> ContentSaverFactory { get; }

        /// <summary>
        /// Фабрика сущностей, сохраняющих снапшоты очереди.
        /// </summary>
        IFactory<IMultiReaderQueueSnapshotSaver<IQueuedUrl>> QueueSnapshotSaverFactory { get; }

        /// <summary>
        /// Сущность, хранящая статистику сбора страниц.
        /// </summary>
        IStatistics Statistics { get; }
    }
}
