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
            public event Action AllWorkersHaveFinished;

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
                Interlocked.Decrement(ref _idleWorkersCount);
            }

            /// <summary>
            /// Метод, проверяющий что все воркеры простаивают.
            /// </summary>
            /// <returns>Если все воркеры простаивают - <see langword="true"/>, иначе - <see langword="false"/>.</returns>
            public bool IsAllWorkersIdle()
            {
                return _idleWorkersCount == Settings.NumberOfWorkers;
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
