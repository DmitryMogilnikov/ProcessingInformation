using System;

namespace WebCrawler.Core.Interfaces
{
    /// <summary>
    /// Интерфейс сущности, сохраняющей текстовое содержимое Web-страниц.
    /// </summary>
    public interface IContentSaver : IDisposable
    {
        /// <summary>
        /// Метод, пытающийся сохранить текстовое содержимое, соответствующий указанному URL-адресу.
        /// </summary>
        /// <param name="url">URL-адрес, которому соответствует текстовое содержимое <paramref name="content"/>.</param>
        /// <param name="timestamp">
        /// Время обращения к ресурсу с адресом <paramref name="url"/>, в результате которого было получено содержимое <paramref name="content"/>.
        /// </param>
        /// <param name="content">Текстовое содержимое, которое требуется сохранить.</param>
        /// <returns>Если удалось сохранить содержимое - <see langword="true"/>, иначе - <see langword="false"/>.</returns>
        bool TrySaveContent(Uri url, DateTime timestamp, string content);
    }
}
