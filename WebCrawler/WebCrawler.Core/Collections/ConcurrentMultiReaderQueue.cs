using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;
using WebCrawler.Core.Models;

namespace WebCrawler.Core.Collections
{
    /// <summary>
    /// Потокобезопасная очередь, распределяющая свои элементы между известным числом читателей.
    /// </summary>
    public class ConcurrentMultiReaderQueue<TValue> : IMultiReaderQueue<TValue> where TValue : notnull
    {
        private const int DefaultBucketInitialCapasity = 100;

        private readonly int _bucketCount;
        private readonly ConcurrentDictionary<int, ConcurrentQueue<TValue>> _buckets;
        // Здесь хотелось бы использовать какой-нибудь ConcurrentHashSet<TValue>, но его в стандартной библиотеке нет,
        // поэтому эмулируем его посредством конкуррентного словаря с минимально возможным размером значений.
        private readonly ConcurrentDictionary<TValue, byte> _alreadyProcessedEntries;

        private readonly ReaderWriterLockSlim _lock = new();

        /// <summary>
        /// Конструктор, позволяющий задать число читателей.
        /// </summary>
        /// <param name="readersCount">Количество читателей.</param>
        /// <param name="equalityComparer">Компаратор для сравнения элементов на равенство.</param>
        public ConcurrentMultiReaderQueue(int readersCount, IEqualityComparer<TValue> equalityComparer)
        {
            _bucketCount = readersCount;
            _buckets = new(concurrencyLevel: readersCount, capacity: readersCount);
            _alreadyProcessedEntries = new(concurrencyLevel: readersCount, 
                                           capacity: DefaultBucketInitialCapasity * readersCount, 
                                           equalityComparer);
        }

        /// <summary>
        /// Конструктор, позволяющий импортировать снапшот очереди.
        /// </summary>
        /// <param name="snapshot">Снапшот очереди.</param>
        /// <param name="equalityComparer">Компаратор для сравнения элементов на равенство.</param>
        /// <remarks>
        /// Число читателей устанавливается таким, какое было у очереди, снапшот которой передан в качестве <paramref name="snapshot"/>.
        /// </remarks>
        public ConcurrentMultiReaderQueue(IMultiReaderQueueSnapshot<TValue> snapshot, IEqualityComparer<TValue> equalityComparer) 
            : this(snapshot.Buckets.Length, snapshot, equalityComparer)
        {
        }

        /// <summary>
        /// Конструктор, позволяющий импортировать снапшот очереди и задать число читателей.
        /// </summary>
        /// <param name="readersCount"></param>
        /// <param name="snapshot"></param>
        /// <param name="equalityComparer"></param>
        public ConcurrentMultiReaderQueue(int readersCount, 
                                          IMultiReaderQueueSnapshot<TValue> snapshot, 
                                          IEqualityComparer<TValue> equalityComparer) 
        {
            _bucketCount = readersCount;
            _buckets = new(concurrencyLevel: readersCount, capacity: readersCount);

            _alreadyProcessedEntries = new(concurrencyLevel: readersCount,
                                           capacity: snapshot.AlreadyProcessedEntries.Count,
                                           equalityComparer);
            foreach (TValue entry in snapshot.AlreadyProcessedEntries)
                _alreadyProcessedEntries.AddOrUpdate(entry, 0, (_, _) => 0);

            TValue[]?[] buckets = snapshot.Buckets;
            // Если число читателей не изменилось с момента снятия снапшота - можно импортировать его как есть.
            if (readersCount == buckets.Length)
            {
                for (int i = 0; i < _bucketCount; i++)
                {
                    TValue[]? bucket = buckets[i];
                    if (bucket is null)
                        continue;

                    ConcurrentQueue<TValue> queue = new(bucket);
                    _buckets.AddOrUpdate(i, queue, (_, _) => queue);
                }
            }
            // Если число читателей изменилось - придётся перераспределять элементы на новое их количество.
            else
            {
                foreach (TValue[]? oldBucket in buckets)
                {
                    if (oldBucket is null)
                        continue;

                    // Мы точно знаем, что во время выполнения конструктора никто не попытается делать блокирующие операции (потому что ещё не с чем),
                    // так что тут нам не нужны все эти блокировки, поэтому вместо Enqueue() используем EnqueueInternal(), чтобы чуть-чуть выиграть в производительности.
                    foreach (TValue value in oldBucket)
                        EnqueueInternal(value);
                }
            }
        }

        /// <summary>
        /// Метод, пытающийся добавить указанное значение в очередь.
        /// </summary>
        /// <param name="value">Значение, которое требуется добавить в очередь.</param>
        /// <returns>
        /// Если в очереди ещё не было элемента <paramref name="value"/> - <see langword="true"/>, иначе - <see langword="false"/>.
        /// </returns>
        public bool TryEnqueue(TValue value)
        {
            bool result = false;

            // Блокировка на чтение несмотря на то, что мы пишем, потому что коллекции конкурентные, и писать мы можем вообще без блокировок.
            // Блокировка нужна только для того, чтобы не менять состояние во время экспорта данных.
            _lock.EnterReadLock();

            if (_alreadyProcessedEntries.TryAdd(value, 0))
            {
                EnqueueInternal(value);
                result = true;
            }

            _lock.ExitReadLock();

            return result;
        }

        /// <summary>
        /// Метод, пытающийся добавить в очередь набор значений.
        /// </summary>
        /// <param name="values">Набор значений, которые требуется добавить в очередь.</param>
        /// <returns>
        /// Набор значений из <paramref name="values"/>, которые уже присутствовали в очереди и НЕ были добавлены 
        /// (или пустая коллекция, если все значения из <paramref name="values"/> были добавлены в очередь).
        /// </returns>
        public IReadOnlyCollection<TValue> TryEnqueueMany(IEnumerable<TValue> values)
        {
            List<TValue> result = new();

            // Ситуация с блокировкой - такая же, как в TryEnqueue().
            _lock.EnterReadLock();

            foreach (TValue value in values)
            {
                if (!_alreadyProcessedEntries.TryAdd(value, 0))
                    result.Add(value);
                else
                    EnqueueInternal(value);
            }

            _lock.ExitReadLock();
            return result;
        }

        /// <summary>
        /// Метод, пытающийся извлечь очередной элемент для читателя с указанным идентификатором.
        /// </summary>
        /// <param name="readerId">Идентификатор (номер по порядку) читателя, элемент для которого требуется извлечь.</param>
        /// <param name="value">Извлечённый элемент или <see langword="null"/>, если элемент не удалось извлечь.</param>
        /// <returns>Если удалось извлечь очередной элемент - <see langword="true"/>, иначе <see langword="false"/>.</returns>
        public bool TryDequeue(int readerId, [NotNullWhen(true)] out TValue? value)
        {
            if (readerId >= _bucketCount)
                throw new ArgumentOutOfRangeException(nameof(readerId));

            _lock.EnterReadLock();

            value = default;
            bool result = _buckets.TryGetValue(readerId, out ConcurrentQueue<TValue>? queue) && queue.TryDequeue(out value);

            _lock.ExitReadLock();
            return result;
        }

        /// <summary>
        /// Метод, экспортирующий текущее состояние очереди.
        /// </summary>
        /// <returns>Снапшот состояния очереди.</returns>
        /// <remarks>На время выполнения этого метода очередь блокируется для операций изменения.</remarks>
        public IMultiReaderQueueSnapshot<TValue> ExportQueueSnapshot()
        {
            // Блокировка на запись, хотя мы читаем - чтобы никто не изменил очередь, пока мы делаем её снапшот.
            _lock.EnterWriteLock();

            TValue[]?[] buckets = new TValue[_bucketCount][];
            for (int i = 0; i < _bucketCount; i++)
            {
                if (!_buckets.TryGetValue(i, out ConcurrentQueue<TValue>? queue))
                    continue;

                buckets[i] = new TValue[queue.Count];
                queue.CopyTo(buckets[i]!, 0);
            }

            IReadOnlyCollection<TValue> alreadyProcessedEntries = _alreadyProcessedEntries.Keys.ToArray();

            _lock.ExitWriteLock();
            return new MultiReaderQueueSnapshot<TValue>(buckets, alreadyProcessedEntries);
        }

        /// <summary>
        /// Метод, проверяющий, что в очереди нет элементов.
        /// </summary>
        /// <returns>Если в очереди нет элементов - <see langword="true"/>, иначе - <see langword="false"/>.</returns>
        /// <remarks>На время выполнения этого метода очередь блокируется для операций изменения.</remarks>
        public bool IsEmpty()
        {
            // Блокировка на запись, хотя мы читаем - чтобы никто не изменил очередь, пока мы проверяем её на пустоту.
            _lock.EnterWriteLock();

            bool result = _buckets.All(bucket => bucket.Value.IsEmpty);

            _lock.ExitWriteLock(); 
            return result;
        }

        /// <summary>
        /// Метод, проверяющий, что для конкретного читателя в очереди нет элементов.
        /// </summary>
        /// <param name="readerId">
        /// Идентификатор (номер по порядку) читателя, наличие элементов для которого требуется проверить.
        /// </param>
        /// <returns>
        /// Если в очереди нет элементов для читателя с идентификатором <paramref name="readerId"/>
        /// - <see langword="true"/>, иначе - <see langword="false"/>.
        /// </returns>
        /// <remarks>Этот метод неблокирующий.</remarks>
        public bool IsBucketEmpty(int readerId)
        {
            if (!_buckets.TryGetValue(readerId, out ConcurrentQueue<TValue>? bucket))
                return true;

            return bucket.IsEmpty;
        }

        /// <summary>
        /// Метод, работающий как <see cref="Enqueue(TValue)"/>, но не берущий блокировку, что позволяет сэкономить в тех ситуациях, где она не нужна.
        /// </summary>
        private void EnqueueInternal(TValue value)
        {
            int bucketNum = Math.Abs(value.GetHashCode()) % _bucketCount;
            ConcurrentQueue<TValue> queue = _buckets.GetOrAdd(bucketNum, _ => new ConcurrentQueue<TValue>());
            queue.Enqueue(value);
        }
    }
}
