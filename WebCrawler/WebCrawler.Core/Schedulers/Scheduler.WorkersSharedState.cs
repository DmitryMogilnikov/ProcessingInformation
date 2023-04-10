using System;
using System.Threading;
using WebCrawler.Core.Collections;
using WebCrawler.Core.Comparers;
using WebCrawler.Core.Interfaces.Models;
using WebCrawler.Core.Interfaces;

namespace WebCrawler.Core.Schedulers
{
    // В этой части класса описан вспомогательный класс, содержащий данные, совместно используемые воркер-потоками.
    public partial class Scheduler
    {
        /// <summary>
        /// Вспомогательный класс, описывающий данные, совместно используемые воркерами, которые запускает планировщик.
        /// </summary>
        private class WorkersSharedState : IDisposable
        {
            public event Action? AllWorkersHaveFinished;

            private readonly ReaderWriterLockSlim _lock = new();
            private CancellationTokenSource _cancellationTokenSource = new();
            private int _requestsCount = 0;
            private int _idleWorkersCount = 0;
            private int _finishedWorkersCount = 0;

            /// <summary>
            /// Очередь URL-адресов страниц для загрузки.
            /// </summary>
            public IMultiReaderQueue<IQueuedUrl> Queue { get; set; }

            /// <summary>
            /// Настройки планировщика запросов.
            /// </summary>
            public ISchedulerSettings Settings { get; }

            /// <summary>
            /// Токен прерывания операций.
            /// </summary>
            public CancellationToken CancellationToken
            {
                get => _cancellationTokenSource.Token;
            }

            /// <summary>
            /// Конструктор.
            /// </summary>
            /// <param name="settings">Настройки планировщика запросов.</param>
            public WorkersSharedState(ISchedulerSettings settings)
            {
                Settings = settings;
                Queue = new ConcurrentMultiReaderQueue<IQueuedUrl>(settings.NumberOfWorkers, QueuedUrlEqualityComparer.Instance);
            }

            /// <summary>
            /// Метод, пытающийся увеличить количество сделанных запросов на 1.
            /// </summary>
            /// <returns>
            /// Если количество запросов достигло предела, задаваемого <see cref="ISchedulerSettings.NumberOfWorkers"/> - 
            /// <see langword="false"/>, иначе - <see langword="true"/>.
            /// </returns>
            /// <remarks>Количество сделанных запросов увеличивается потокобезопасно и неблокирующим образом.</remarks>
            public bool TryIncrementRequestsCount()
            {
                int currentValue;
                int exchangedValue;
                do
                {
                    currentValue = _requestsCount;
                    if (currentValue >= Settings.RequestsPerSecond)
                        return false;

                    exchangedValue = Interlocked.CompareExchange(ref _requestsCount, currentValue + 1, currentValue);
                } while (exchangedValue != currentValue);
                return true;
            }

            /// <summary>
            /// Метод, сбрасывающий количество сделанных запросов.
            /// </summary>
            public void ResetRequestsCount()
            {
                _requestsCount = 0;
            }

            /// <summary>
            /// Метод, атомарно инкрементирующий количество простаивающих воркеров.
            /// </summary>
            public void IncrementIdleWorkersCount()
            {
                Interlocked.Increment(ref _idleWorkersCount);
            }

            /// <summary>
            /// Метод, атомарно декрементирующий количество простаивающих воркеров.
            /// </summary>
            public void DecrementIdleWorkersCount()
            {
                // Блокировка на чтение, хотя мы изменяем значение потому, что декремент атомарный и для него самого блокировка не нужна.
                // Она здесь для того, чтобы при проверке того, что вся работа сделана, какой-нибудь воркер не поменял число
                // простаивающих воркеров ПОСЛЕ его проверки, но ДО проверки пустоты очереди. В этом случае эксклюзивная блокировка для изменения значения не нужна.
                // При инкременте блокировка не нужна, т.к. несвоевременный инкремент лишь заставит воркер прождать один лишний цикл, что не страшно.
                _lock.EnterReadLock();

                Interlocked.Decrement(ref _idleWorkersCount);

                _lock.ExitReadLock();
            }

            /// <summary>
            /// Метод, проверяющий что все воркеры простаивают, а очередь пуста.
            /// </summary>
            /// <returns>Если все воркеры простаивают и очередь пуста - <see langword="true"/>, иначе - <see langword="false"/>.</returns>
            public bool IsAllWorkDone()
            {
                // Блокировка на запись, хотя мы читаем, потому что важно запретить изменение, а при изменении мы берём блокировку на чтение, как описано выше.
                _lock.EnterWriteLock();

                // Если все воркеры в данный момент простаивают - это может значить, что работа действительно закончилась, но не обязательно -
                // возможно, какой-то воркер успел подложить работы другому, прежде простаивающему, и уйти в простой, а этот другой ещё не успел проверить наличие для него работы и выйти из простоя.
                // Чтобы исключить такую ситуацию, дополнительно проверяем, что не только все воркеры простаивают, но и очередь пуста.
                // Мы проверяем пустоту очереди только в тех случаях, когда все воркеры простаивают, потому что для ConcurrentMultiReaderQueue эта операция полностью блокирующая,
                // а значит - дорогая, т.к. заставит ждать всех остальных воркеров. Но если они и так (предположительно) ничего не делают - то можно.
                bool result = _idleWorkersCount == Settings.NumberOfWorkers && Queue.IsEmpty();

                _lock.ExitWriteLock();
                return result;
            }

            /// <summary>
            /// Метод, инкрементирующий количество воркеров, закончивших работу.
            /// </summary>
            public void IncrementFinishedWorkersCount()
            {
                Interlocked.Increment(ref _finishedWorkersCount);
                if (_finishedWorkersCount == Settings.NumberOfWorkers)
                    AllWorkersHaveFinished?.Invoke();
            }

            /// <summary>
            /// Метод, устанавливающий <see cref="CancellationToken"/> в состояние запроса прерывания операции.
            /// </summary>
            public void RequestCancellation()
            {
                _cancellationTokenSource.Cancel();
            }

            /// <summary>
            /// Метод, сбрасывающий состояние <see cref="WorkersSharedState"/>.
            /// </summary>
            /// <remarks>Этот метод не затрагивает ни настройки планировщика, ни текущее состояние очереди.</remarks>
            public void ResetState()
            {
                ResetRequestsCount();
                _idleWorkersCount = 0;
                _finishedWorkersCount = 0;
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }

            /// <summary>
            /// Метод, освобождающий используемые неуправляемые ресурсы.
            /// </summary>
            public void Dispose()
            {
                _cancellationTokenSource.Dispose();
            }
        }
    }
}
