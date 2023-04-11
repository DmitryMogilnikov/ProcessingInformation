using System;
using System.IO;
using System.Text.Json;
using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Readers
{
    /// <summary>
    /// Читатель сохранённых снапшотов очереди с несколькими читателями из файлов.
    /// </summary>
    /// <typeparam name="TQueuedValue">Тип элементов очереди.</typeparam>
    /// <typeparam name="TSnapshot">Конкретный тип читаемого снапшота.</typeparam>
    public class FileMultiReaderQueueSnapshotReader<TQueuedValue, TSnapshot> : IMultiReaderQueueSnapshotReader<string, TQueuedValue, TSnapshot>
        where TQueuedValue : notnull
        where TSnapshot : notnull, IMultiReaderQueueSnapshot<TQueuedValue>
    {
        /// <summary>
        /// Метод, читающий сохранённый снапшот очереди с несколькими читателями.
        /// </summary>
        /// <param name="source">Путь к файлу, из которого требуется прочитать снапшот.</param>
        /// <returns>Прочитанный снапшот</returns>
        /// <exception cref="FileNotFoundException">Не удалось найти файл с сохранённым снапшотом.</exception>
        /// <exception cref="Exception">Не удалось прочитать снапшот из указанного файла.</exception>
        public TSnapshot ReadSnapshot(string source)
        {
            if (!Path.Exists(source))
                throw new FileNotFoundException("Не удалось найти файл с сохранённым снапшотом.", source);

            using FileStream file = File.OpenRead(source);
            // Записываемый снапшот всегда не null, так что если прочитался null - что-то точно пошло не так.
            // TODO: Уточнить тип исключения.
            return JsonSerializer.Deserialize<TSnapshot>(file) ?? throw new Exception($"Не удалось прочитать снапшот из файла {source}.");
        }
    }
}
