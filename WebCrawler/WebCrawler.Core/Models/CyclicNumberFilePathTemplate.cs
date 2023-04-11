using System.Threading;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Models
{
    /// <summary>
    /// Класс, описывающий шаблон пути к файлу, в который циклически подставляется номер пути по порядку.
    /// </summary>
    /// <remarks>Этот класс потокобезопасен.</remarks>
    public class CyclicNumberFilePathTemplate : IFilePathTemplate
    {
        /// <summary>
        /// Максимальное количество уникальных путей по умолчанию.
        /// </summary>
        public const int DefaultMaxFilesCount = 10;

        private uint _requestedPathsCount = 0;

        /// <summary>
        /// Путь к папке, в которой должны содержаться файлы.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Шаблон имени файла. 
        /// Может содержать маркер подстановки - в таком случае вместо него будет подставляться очередной номер
        /// (циклически; размер цикла определяется свойством <see cref="MaxFilesCount"/>).
        /// </summary>
        public string FileNameTemplate { get; }

        /// <summary>
        /// Максимальное количество уникальных путей. После исчерпания - пути, 
        /// возвращаемые методом <see cref="GetNextFilePath"/> начнут циклически повторяться.
        /// </summary>
        public int MaxFilesCount { get; }

        /// <summary>
        /// Конструктор с максимальным количествоv уникальных путей по умолчанию.
        /// </summary>
        /// <param name="path">Путь к папке, в которой должны содержаться файлы.</param>
        /// <param name="fileNameTemplate">Шаблон имени файла. Может содержать маркер подстановки.</param>
        public CyclicNumberFilePathTemplate(string path, string fileNameTemplate) : this(path, fileNameTemplate, DefaultMaxFilesCount)
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="path">Путь к папке, в которой должны содержаться файлы.</param>
        /// <param name="fileNameTemplate">Шаблон имени файла. Может содержать маркер подстановки.</param>
        /// <param name="maxFilesCount">Максимальное количество уникальных путей.</param>
        public CyclicNumberFilePathTemplate(string path, string fileNameTemplate, int maxFilesCount)
        {
            Path = path;
            FileNameTemplate = fileNameTemplate;
            MaxFilesCount = maxFilesCount;
        }

        /// <summary>
        /// Метод, возвращающий очередной путь, соответствующий шаблону.
        /// </summary>
        /// <remarks>
        /// Если в <see cref="FileNameTemplate"/> присутствует маркер подстановки - вместо него будет подставлен 
        /// номер по порядку вызова метода <see cref="GetNextFilePath"/> по модулю <see cref="MaxFilesCount"/>.<br/>
        /// При большом количестве вызовов (> 2^32) возможно нарушение цикличности из-за переполнения типа <see cref="uint"/>,
        /// используемого для хранения количества вызовов метода.
        /// </remarks>
        public string GetNextFilePath()
        {
            // Считаем, что переполнение - это нормально, т.к. такое поведение задокументировано.
            // Строго говоря, Interlocked.Increment в любом случае не проверяет переполнение, но для наглядности - обернём в unchecked обе строки.
            unchecked
            {
                uint requestedPathCount = Interlocked.Increment(ref _requestedPathsCount);
                return ((requestedPathCount - 1) % MaxFilesCount).ToString();
            }
        }
    }
}
