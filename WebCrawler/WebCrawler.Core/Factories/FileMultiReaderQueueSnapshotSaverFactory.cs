using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;
using WebCrawler.Core.Savers;

namespace WebCrawler.Core.Factories
{
    /// <summary>
    /// Фабрика объектов класса, сохраняющего снапшоты очереди с несколькими читателями в файл.
    /// </summary>
    /// <typeparam name="TValue">Тип элементов очереди.</typeparam>
    public class FileMultiReaderQueueSnapshotSaverFactory<TValue> : IFactory<IMultiReaderQueueSnapshotSaver<TValue>>
        where TValue : notnull
    {
        private readonly IFilePathTemplate _filePathTemplate;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="filePathTemplate">Шаблон пути к файлам, в которые будут сохраняться снапшоты.</param>
        public FileMultiReaderQueueSnapshotSaverFactory(IFilePathTemplate filePathTemplate)
        {
            _filePathTemplate = filePathTemplate;
        }

        /// <summary>
        /// Метод, возвращающий новый экземпляр класса, сохраняющего снапшоты очереди с несколькими читателями в файл.
        /// </summary>
        /// <remarks>Все экземпляры разделяют один и тот же шаблон пути к файлам.</remarks>
        public IMultiReaderQueueSnapshotSaver<TValue> Create()
        {
            return new FileMultiReaderQueueSnapshotSaver<TValue>(_filePathTemplate);
        }
    }
}
