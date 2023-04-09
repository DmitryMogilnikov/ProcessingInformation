using System;
using System.Collections.Generic;

namespace WebCrawler.Core.Interfaces.Models
{
    /// <summary>
    /// Интерфейс содержимого Web-страницы.
    /// </summary>
    public interface IPageContent
    {
        /// <summary>
        /// Текстовое содержимое Web-страницы.
        /// </summary>
        string TextContent { get; }

        /// <summary>
        /// Список ссылок, присутствующих на Web-странице.
        /// </summary>
        IEnumerable<Uri> Links { get; }
    }
}
