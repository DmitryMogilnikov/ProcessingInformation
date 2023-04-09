using System;
using System.Collections.Generic;
using WebCrawler.Core.Interfaces.Models;

namespace WebCrawler.Core.Models
{
    /// <summary>
    /// Рекорд, описывающий содержимое Web-страницы.
    /// </summary>
    /// <param name="TextContent">Текстовое содержимое Web-страницы.</param>
    /// <param name="Links">Список ссылок, присутствующих на Web-странице.</param>
    public record PageContent(string TextContent, IEnumerable<Uri> Links) : IPageContent;
}
