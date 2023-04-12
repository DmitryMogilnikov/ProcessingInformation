using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WebCrawler.Core.Interfaces;

namespace WebCrawler.Core.Savers
{
    /// <summary>
    /// Класс, сохраняющий текстовое содержимое Web-страниц в файл в формате CSV.
    /// </summary>
    /// <remarks>Методы этого класса потокобезопасны.</remarks>
    public class ConcurrentCsvContentSaver : IContentSaver, IDisposable
    {
        /// <summary>
        /// Размер внутреннего буфера по умолчанию.
        /// </summary>
        public const int DefaultBufferSize = 50;

        private readonly string _filePath;
        private readonly int _bufferSize;

        private readonly ConcurrentQueue<string> _queue = new();
        private readonly object _createWriterTaskLock = new();
        private readonly object _writeToFileLock = new();
        private Task? _fileWriter;

        /// <summary>
        /// Конструктор с размером внутреннего буфера по умолчанию.
        /// </summary>
        /// <param name="filePath">Путь к файлу, в который требуется сохранять содержимое.</param>
        public ConcurrentCsvContentSaver(string filePath) : this(filePath, DefaultBufferSize) 
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="filePath">Путь к файлу, в который требуется сохранять содержимое.</param>
        /// <param name="bufferSize">Размер внутреннего буфера. После его заполнения начинается запись в файл.</param>
        public ConcurrentCsvContentSaver(string filePath, int bufferSize)
        {
            _filePath = filePath;
            _bufferSize = bufferSize;
        }

        /// <summary>
        /// Метод, пытающийся сохранить текстовое содержимое, соответствующий указанному URL-адресу.
        /// </summary>
        /// <param name="url">URL-адрес, которому соответствует текстовое содержимое <paramref name="content"/>.</param>
        /// <param name="timestamp">
        /// Время обращения к ресурсу с адресом <paramref name="url"/>, в результате которого было получено содержимое <paramref name="content"/>.
        /// </param>
        /// <param name="content">Текстовое содержимое, которое требуется сохранить.</param>
        /// <returns>Всегда возвращает <see langword="true"/>.</returns>
        public bool TrySaveContent(Uri url, DateTime timestamp, string content)
        {
            string csvEntry = CreateCSVEntry(url, timestamp, content);
            _queue.Enqueue(csvEntry);
            SaveActiveQueueIfNeeded();
            return true;
        }

        /// <summary>
        /// Метод, принудительно записывающий содержимое буфера в файл.
        /// </summary>
        public void Dispose()
        {
            SaveQueue();
        }

        private void SaveActiveQueueIfNeeded()
        {
            if (_queue.Count < _bufferSize)
                return;

            if (_fileWriter is not null && !_fileWriter.IsCompleted)
                return;

            lock (_createWriterTaskLock)
            {
                if (_queue.Count < _bufferSize)
                    return;

                if (_fileWriter is not null && !_fileWriter.IsCompleted)
                    return;

                _fileWriter = Task.Run(SaveQueue);
            }
        }

        private void SaveQueue()
        {
            try
            {
                lock (_writeToFileLock)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_filePath) ?? "/");

                    using StreamWriter writer = new(_filePath, true);
                    while (_queue.TryDequeue(out string? entry))
                    {
                        writer.WriteLine(entry);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static string CreateCSVEntry(Uri url, DateTime timestamp, string content)
        {
            string urlString = EscapeCSVValue(url.ToString());
            string timestampString = EscapeCSVValue(timestamp.ToString("o"));
            string contentString = EscapeCSVValue(content.Replace("\n", ""));
            return $"{urlString},{timestampString},{contentString}";
        }

        private static string EscapeCSVValue(string value)
        {
            string escapedValue = value.Replace("\"", "\"\"");
            return $"\"{escapedValue}\"";
        }
    }
}
