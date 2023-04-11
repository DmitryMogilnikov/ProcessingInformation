using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Interfaces
{
    /// <summary>
    /// Интерфейс читателя сохранённых снапшотов очереди с несколькими читателями.
    /// </summary>
    /// <typeparam name="TSource">Тип параметра, задающего источник снапшота.</typeparam>
    /// <typeparam name="TQueueValue">Тип элементов очереди.</typeparam>
    public interface IMultiReaderQueueSnapshotReader<in TSource, out TQueueValue, out TSnapshot> 
        where TSource : notnull
        where TQueueValue : notnull
        where TSnapshot : notnull, IMultiReaderQueueSnapshot<TQueueValue>
    {
        /// <summary>
        /// Метод, читающий сохранённый снапшот очереди с несколькими читателями.
        /// </summary>
        /// <param name="source">Параметр, каким-либо образом задающий источник, из которого требуется прочитать снапшот.</param>
        /// <returns>Прочитанный снапшот</returns>
        TSnapshot ReadSnapshot(TSource source);
    }
}
