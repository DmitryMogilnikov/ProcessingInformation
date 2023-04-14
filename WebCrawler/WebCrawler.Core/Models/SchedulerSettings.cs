using System;
using System.Collections.Generic;
using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Models
{
    /// <summary>
    /// Рекорд, описывающий настройки планировщика запросов.
    /// </summary>
    /// <param name="NumberOfWorkers">Количество загрузчиков, работающих одновременно.</param>
    /// <param name="RequestsPerSecond">Максимальное суммарное количество запросов в секунду.</param>
    /// <param name="MaxRetries">Максимальное количество повторных попыток запросить страницу.</param>
    /// <param name="QueueSnapshotSavePeriod">Частота сохранения снапшота очереди.</param>
    /// <param name="DataGetterFactories">Упорядоченный в порядке уменьшения приоритета набор фабрик получателей данных.</param>
    /// <param name="ContentParserFactories">Упорядоченный в порядке уменьшения приоритета набор фабрик парсеров данных в содержимое страницы.</param>
    /// <param name="ContentSaverFactory">Фабрика сущностей, сохраняющих содержимое Web-страниц.</param>
    /// <param name="QueueSnapshotSaverFactory">Фабрика сущностей, сохраняющих снапшоты очереди.</param>
    /// <param name="Statistics">Сущность, хранящая статистику сбора страниц.</param>
    public record SchedulerSettings(int NumberOfWorkers,
                                    int RequestsPerSecond,
                                    int MaxRetries,
                                    TimeSpan QueueSnapshotSavePeriod,
                                    IEnumerable<IFactory<IDataGetter>> DataGetterFactories,
                                    IEnumerable<IFactory<IContentParser>> ContentParserFactories,
                                    IFactory<IContentSaver> ContentSaverFactory,
                                    IFactory<IMultiReaderQueueSnapshotSaver<IQueuedUrl>> QueueSnapshotSaverFactory,
                                    IStatistics Statistics)
        : ISchedulerSettings
    {
        /// <summary>
        /// Количество загрузчиков, работающих одновременно по умолчанию.
        /// </summary>
        public const int DefaultNumberOfWorkes = 1;

        /// <summary>
        /// Максимальное суммарное количество запросов в секунду по умолчанию.
        /// </summary>
        public const int DefaultRequestsPerSecond = 100;

        /// <summary>
        /// Максимальное количество повторных попыток запросить страницу по умолчанию.
        /// </summary>
        public const int DefaultMaxRetries = 1;

        /// <summary>
        /// Частота сохранения снапшота очереди по умолчанию.
        /// </summary>
        public static readonly TimeSpan DefaultQueueSnapshotSavePeriod = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Конструктор, создающий настройки планировщика запросов со значениями по умолчанию.
        /// </summary>
        /// <param name="dataGetterFactories">Упорядоченный в порядке уменьшения приоритета набор фабрик получателей данных.</param>
        /// <param name="contentParserFactories">Упорядоченный в порядке уменьшения приоритета набор фабрик парсеров данных в содержимое страницы.</param>
        /// <param name="contentSaverFactory">Фабрика сущностей, сохраняющих содержимое Web-страниц.</param>
        /// <param name="queueSnapshotSaverFactory">Фабрика сущностей, сохраняющих снапшоты очереди.</param>
        /// <param name="statistics">Сущность, хранящая статистику сбора страниц.</param>
        public SchedulerSettings(IEnumerable<IFactory<IDataGetter>> dataGetterFactories,
                                 IEnumerable<IFactory<IContentParser>> contentParserFactories,
                                 IFactory<IContentSaver> contentSaverFactory,
                                 IFactory<IMultiReaderQueueSnapshotSaver<IQueuedUrl>> queueSnapshotSaverFactory,
                                 IStatistics statistics) 
            : this(DefaultNumberOfWorkes,
                   DefaultRequestsPerSecond,
                   DefaultMaxRetries,
                   DefaultQueueSnapshotSavePeriod,
                   dataGetterFactories,
                   contentParserFactories,
                   contentSaverFactory,
                   queueSnapshotSaverFactory,
                   statistics)
        {
        }
    }
}
