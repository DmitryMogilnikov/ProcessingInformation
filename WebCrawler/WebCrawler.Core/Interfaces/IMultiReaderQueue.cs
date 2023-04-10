using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Interfaces
{
    /// <summary>
    /// Интерфейс очереди, содержащей уникальные значения и распределяющей свои элементы между известным числом читателей.
    /// </summary>
    /// <typeparam name="TValue">Тип элементов очереди.</typeparam>
    public interface IMultiReaderQueue<TValue> where TValue : notnull
    {
        /// <summary>
        /// Метод, пытающийся добавить указанное значение в очередь.
        /// </summary>
        /// <param name="value">Значение, которое требуется добавить в очередь.</param>
        /// <returns>
        /// Если в очереди ещё не было элемента <paramref name="value"/> - <see langword="true"/>, иначе - <see langword="false"/>.
        /// </returns>
        bool TryEnqueue(TValue value);

        /// <summary>
        /// Метод, пытающийся добавить в очередь набор значений.
        /// </summary>
        /// <param name="values">Набор значений, которые требуется добавить в очередь.</param>
        /// <returns>
        /// Набор значений из <paramref name="values"/>, которые уже присутствовали в очереди и НЕ были добавлены 
        /// (или пустая коллекция, если все значения из <paramref name="values"/> были добавлены в очередь).
        /// </returns>
        IReadOnlyCollection<TValue> TryEnqueueMany(IEnumerable<TValue> values);

        /// <summary>
        /// Метод, пытающийся извлечь очередной элемент для читателя с указанным идентификатором.
        /// </summary>
        /// <param name="readerId">Идентификатор (номер по порядку) читателя, элемент для которого требуется извлечь.</param>
        /// <param name="value">Извлечённый элемент или <see langword="null"/>, если элемент не удалось извлечь.</param>
        /// <returns>Если удалось извлечь очередной элемент - <see langword="true"/>, иначе <see langword="false"/>.</returns>
        bool TryDequeue(int readerId, [NotNullWhen(true)] out TValue? value);

        /// <summary>
        /// Метод, экспортирующий текущее состояние очереди.
        /// </summary>
        /// <returns>Снапшот состояния очереди.</returns>
        IMultiReaderQueueSnapshot<TValue> ExportQueueSnapshot();

        /// <summary>
        /// Метод, проверяющий, что в очереди нет элементов.
        /// </summary>
        /// <returns>Если в очереди нет элементов - <see langword="true"/>, иначе - <see langword="false"/>.</returns>
        bool IsEmpty();

        /// <summary>
        /// Метод, проверяющий, что для конкретного читателя в очереди нет элементов.
        /// </summary>
        /// <param name="readerId">
        /// Идентификатор (номер по порядку) читателя, наличие элементов для которого требуется проверить.
        /// </param>
        /// <returns>
        /// Если в очереди нет элементов для читателя с идентификатором <paramref name="readerId"/>
        /// - <see langword="true"/>, иначе - <see langword="false"/>.
        /// </returns>
        bool IsBucketEmpty(int readerId);
    }
}
