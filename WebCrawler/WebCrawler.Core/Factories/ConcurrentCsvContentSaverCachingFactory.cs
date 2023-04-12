using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Savers;

namespace WebCrawler.Core.Factories
{
    /// <summary>
    /// Фабрика объектов класса, сохраняющего текстовое содержимое Web-страниц в файл в формате CSV.
    /// </summary>
    public class ConcurrentCsvContentSaverCachingFactory : IFactory<IContentSaver>
    {
        private readonly IContentSaver _contentSaver;

        /// <summary>
        /// Конструктор, использующий для класса, сохраняющего содержимое, размер буфера по умолчанию.
        /// </summary>
        /// <param name="filePath">Путь к файлу, в который требуется сохранять содержимое.</param>
        public ConcurrentCsvContentSaverCachingFactory(string filePath)
        {
            _contentSaver = new ConcurrentCsvContentSaver(filePath);
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="filePath">Путь к файлу, в который требуется сохранять содержимое.</param>
        /// <param name="bufferSize">Размер внутреннего буфера класса, сохраняющего содержимое. После его заполнения начинается запись в файл.</param>
        public ConcurrentCsvContentSaverCachingFactory(string filePath, int bufferSize)
        {
            _contentSaver = new ConcurrentCsvContentSaver(filePath, bufferSize);
        }

        /// <summary>
        /// Метод, возвращающий экземпляр класса, сохраняющего текстовое содержимое Web-страниц в файл в формате CSV.
        /// </summary>
        /// <remarks>Этот метод всегда возвращает один и тот же экземпляр класса.</remarks>
        IContentSaver IFactory<IContentSaver>.Create()
        {
            return _contentSaver;
        }
    }
}
