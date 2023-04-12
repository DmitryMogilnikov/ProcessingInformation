using System;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Models
{
    /// <summary>
    /// Класс, описывающий шаблон пути к файлу, в который подставляется текущее время в формате "yyyy-MM-ddTHH_mm_ss".
    /// </summary>
    public class TimestampFilePathTemplate : IFilePathTemplate
    {
        /// <summary>
        /// Путь к папке, в которой должны содержаться файлы.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Шаблон имени файла. 
        /// Может содержать маркер подстановки - в таком случае вместо него будет подставляться время вызова метода <see cref="GetNextFilePath"/>.
        /// </summary>
        public string FileNameTemplate { get; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="path">Путь к папке, в которой должны содержаться файлы.</param>
        /// <param name="fileName">Шаблон имени файла. Может содержать маркер подстановки.</param>
        public TimestampFilePathTemplate(string path, string fileName)
        {
            Path = path;
            FileNameTemplate = fileName;
        }

        /// <summary>
        /// Метод, возвращающий очередной путь, соответствующий шаблону.
        /// </summary>
        /// <remarks>
        /// Если в <see cref="FileNameTemplate"/> присутствует маркер подстановки -
        /// вместо него будет подставлен момент вызова метода <see cref="GetNextFilePath"/> в формате "yyyy-MM-ddTHH_mm_ss".
        /// </remarks>
        public string GetNextFilePath()
        {
            string fileName = string.Format(FileNameTemplate, DateTime.UtcNow.ToString("yyyy-MM-ddTHH_mm_ss"));
            return System.IO.Path.Combine(Path, fileName);
        }
    }
}
