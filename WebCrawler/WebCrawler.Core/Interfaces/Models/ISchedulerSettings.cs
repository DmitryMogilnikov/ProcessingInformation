using System;

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
        /// Фабрика загрузчиков содержимого Web-страниц.
        /// </summary>
        IFactory<IWebDownloader> WebDownloaderFactory { get; }

        /// <summary>
        /// Фабрика сущностей, сохраняющих содержимое Web-страниц.
        /// </summary>
        IFactory<IContentSaver> ContentSaverFactory { get; }

        /// <summary>
        /// Фабрика сущностей, сохраняющих снапшоты очереди.
        /// </summary>
        IFactory<IMultiReaderQueueSnapshotSaver<IQueuedUrl>> QueueSnapshotSaverFactory { get; }
    }
}
