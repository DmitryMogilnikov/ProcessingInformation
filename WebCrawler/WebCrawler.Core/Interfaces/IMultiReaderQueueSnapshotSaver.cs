using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Interfaces
{
    /// <summary>
    /// Интерфейс сущности, сохраняющей снапшоты очереди с несколькими читателями.
    /// </summary>
    /// <typeparam name="TValue">Тип элементов очереди.</typeparam>
    public interface IMultiReaderQueueSnapshotSaver<in TValue> where TValue : notnull
    {
        /// <summary>
        /// Метод, пытающийся сохранить снапшот очереди с несколькими читателями.
        /// </summary>
        /// <param name="snapshot">Снапшот очереди, который требуется сохранить.</param>
        /// <returns>
        /// Если удалось сохранить снапшот <paramref name="snapshot"/> - <see langword="true"/>, иначе - <see langword="false"/>.
        /// </returns>
        bool TrySaveSnapshot(IMultiReaderQueueSnapshot<TValue> snapshot);
    }
}
