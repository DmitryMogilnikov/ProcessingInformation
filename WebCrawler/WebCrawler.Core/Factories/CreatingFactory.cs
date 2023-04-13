using WebCrawler.Core.Interfaces;

namespace WebCrawler.Core.Factories
{
    /// <summary>
    /// Фабрика, создающая объекты указанного типа путём вызова конструктора по умолчанию.
    /// </summary>
    /// <typeparam name="T">Тип создаваемых объектов.</typeparam>
    public class CreatingFactory<T> : IFactory<T> where T : notnull, new()
    {
        /// <summary>
        /// Метод, создающий новый объект типа <typeparamref name="T"/> путём вызова конструктора по умолчанию.
        /// </summary>
        /// <returns>Новый объект типа <typeparamref name="T"/>.</returns>
        public T Create()
        {
            return new();
        }
    }
}
