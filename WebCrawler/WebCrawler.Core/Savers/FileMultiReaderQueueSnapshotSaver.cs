using System.Diagnostics;
using System;
using System.IO;
using System.Text.Json;
using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Savers
{
    /// <summary>
    /// Класс, сохраняющий снапшоты очереди с несколькими читателями в файлы.
    /// </summary>
    /// <typeparam name="TValue">Тип элементов очереди.</typeparam>
    public class FileMultiReaderQueueSnapshotSaver<TValue> : IMultiReaderQueueSnapshotSaver<TValue> where TValue : notnull
    {
        private readonly IFilePathTemplate _filePathTemplate;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="filePathTemplate">Шаблон пути к файлам, в которые будут сохраняться снапшоты.</param>
        public FileMultiReaderQueueSnapshotSaver(IFilePathTemplate filePathTemplate)
        {
            _filePathTemplate = filePathTemplate;
        }

        /// <summary>
        /// Метод, пытающийся сохранить снапшот очереди с несколькими читателями.
        /// </summary>
        /// <param name="snapshot">Снапшот очереди, который требуется сохранить.</param>
        /// <returns>
        /// Если удалось сохранить снапшот <paramref name="snapshot"/> - <see langword="true"/>, иначе - <see langword="false"/>.
        /// </returns>
        public bool TrySaveSnapshot(IMultiReaderQueueSnapshot<TValue> snapshot)
        {
            string filePath = _filePathTemplate.GetNextFilePath();
            try
            {
                // Создаём иерархию папок, если она ещё не создана.
                Directory.CreateDirectory(_filePathTemplate.Path);

                using FileStream file = File.Create(filePath);
                // Здесь бы сериализовать во что-то бинарное, но в стандартной библиотеке бинарная сериализация объявлена небезопасной и устаревшей,
                // а свою мне писать лень, так что пока что обойдёмся JSON-ом.
                JsonSerializer.Serialize(file, snapshot);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}
