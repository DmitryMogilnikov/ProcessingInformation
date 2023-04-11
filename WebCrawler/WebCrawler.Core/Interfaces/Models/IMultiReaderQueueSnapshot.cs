using System.Collections.Generic;

namespace WebCrawler.Core.Interfaces.Models
{
    /// <summary>
    /// Интерфейс снапшота состояния очереди с несколькими читателями.
    /// </summary>
    /// <typeparam name="TValue">Тип элементов очереди.</typeparam>
    public interface IMultiReaderQueueSnapshot<out TValue> where TValue : notnull
    {
        /// <summary>
        /// Набор элементов очереди, распределённых по читателям.
        /// </summary>
        TValue[]?[] Buckets { get; }

        /// <summary>
        /// Набор элементов, которые когда-либо были добавлены в очередь.
        /// </summary>
        IReadOnlyCollection<TValue> AlreadyProcessedEntries { get; }
    }
}
