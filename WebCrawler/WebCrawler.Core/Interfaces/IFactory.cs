namespace WebCrawler.Core.Interfaces
{
    /// <summary>
    /// Интерфейс фабрики каких-либо сущностей.
    /// </summary>
    /// <typeparam name="T">Тип сущностей, создаваемых фабрикой.</typeparam>
    public interface IFactory<out T> where T : notnull
    {
        /// <summary>
        /// Метод, создающий новую сущность.
        /// </summary>
        /// <returns>Новая сущность, созданная фабрикой.</returns>
        T Create();
    }
}
