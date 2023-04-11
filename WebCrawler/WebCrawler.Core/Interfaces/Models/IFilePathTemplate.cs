namespace WebCrawler.Core.Interfaces.Models
{
    /// <summary>
    /// Интерфейс шаблона пути к файлу.
    /// </summary>
    public interface IFilePathTemplate
    {
        /// <summary>
        /// Путь к папке, в которой должны содержаться файлы.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Шаблон имени файла.
        /// </summary>
        /// <remarks>Может содержать некоторое зависящее от конкретной реализации маркеров подстановки.</remarks>
        string FileNameTemplate { get; }

        /// <summary>
        /// Метод, возвращающий очередной путь, соответствующий шаблону.
        /// </summary>
        string GetNextFilePath();
    }
}
