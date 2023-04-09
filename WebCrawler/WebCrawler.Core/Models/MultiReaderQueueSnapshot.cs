using System.Collections.Generic;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Models
{
    /// <summary>
    /// Рекорд, описывающий снапшот состояния очереди с несколькими читателями.
    /// </summary>
    /// <typeparam name="TValue">Тип элементов очереди.</typeparam>
    /// <param name="Buckets">Набор элементов очереди, распределённых по читателям.</param>
    /// <param name="AlreadyProcessedEntries">Набор элементов, которые когда-либо были добавлены в очередь.</param>
    public record MultiReaderQueueSnapshot<TValue>(TValue[]?[] Buckets, IReadOnlyCollection<TValue> AlreadyProcessedEntries) 
        : IMultiReaderQueueSnapshot<TValue> where TValue : notnull;
}
