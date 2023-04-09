using System;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Interfaces
{
    /// <summary>
    /// Интерфейс планировщика запросов.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Настройки планировщика запросов.
        /// </summary>
        ISchedulerSettings Settings { get; }

        /// <summary>
        /// Метод, импортироующий снапшот очереди URL-адресов.
        /// </summary>
        /// <param name="snapshot">Снапшот, который требуется импортировать.</param>
        /// <remarks>Этот метод заменяет текущую очередь на очередь из снапшота.</remarks>
        void ImportQueueSnapshot(IMultiReaderQueueSnapshot<IQueuedUrl> snapshot);

        /// <summary>
        /// Метод, экспортирующий снапшот текущего состояния очереди URL-адресов.
        /// </summary>
        /// <returns>Снапшот текущего состояния очереди URL-запросов.</returns>
        IMultiReaderQueueSnapshot<IQueuedUrl> ExportQueueSnapshot();

        /// <summary>
        /// Метод, добавляющий URL-адреса в очередь.
        /// </summary>
        /// <param name="urls">Один или несколько URL-адресов, доторые требуется добавить в очередь.</param>
        void AddUrls(params Uri[] urls);

        /// <summary>
        /// Метод, запускающий или возобновляющий процесс сбора страниц.
        /// </summary>
        void Start();

        /// <summary>
        /// Метод, принудительно приостанавливающий процесс сбора страниц.
        /// </summary>
        public void Stop();
    }
}
