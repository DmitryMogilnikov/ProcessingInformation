using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Core.Collections;
using WebCrawler.Core.Comparers;
using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;
using WebCrawler.Core.Models;

namespace WebCrawler.Core.Schedulers
{
    /// <summary>
    /// Планировщик запросов.
    /// </summary>
    // Это основная часть класса с базовой логикой и, в том числе, реализацией публичного интерфейса.
    // Другие части содержат только отдельные аспекты внутренней логики.
    public partial class Scheduler : IScheduler, IDisposable
    {
        /// <summary>
        /// Событие, происходящее когда содержимое всех страниц из очереди было собрано.
        /// </summary>
        public event Action? AllPagesCollected;

        private readonly WorkersSharedState _workersSharedState;
        private Task[]? _workers;
        private readonly object _startLock = new();

        /// <summary>
        /// Настройки планировщика запросов.
        /// </summary>
        public ISchedulerSettings Settings
        {
            get => _workersSharedState.Settings;
        }

        /// <summary>
        /// Признак того, что процесс сбора страниц в данный момент запущен.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_workers))]
        public bool IsStarted { get; private set; } = false;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="settings">Настройки планировщика запросов.</param>
        public Scheduler(ISchedulerSettings settings)
        {
            _workersSharedState = new WorkersSharedState(settings);
            // Мы не можем обрабатывать событие в потоке, который его вызвал, т.к. это приведёт к дедлоку -
            // обработчик события будет ждать завершения в т.ч. и того потока, в котором выполняется он сам.
            // Поэтому при возникновении события будем создавать новый поток и обрабатывать событие в нём.
            _workersSharedState.AllWorkersHaveFinished += () => Task.Run(() => Stop(isExternalReason: false));
        }

        /// <summary>
        /// Метод, импортирующий снапшот очереди URL-адресов.
        /// </summary>
        /// <param name="snapshot">Снапшот, который требуется импортировать.</param>
        /// <remarks>Этот метод заменяет текущую очередь на очередь из снапшота.</remarks>
        /// <exception cref="InvalidOperationException">Процесс сбора страниц уже запущен.</exception>
        public void ImportQueueSnapshot(IMultiReaderQueueSnapshot<IQueuedUrl> snapshot)
        {
            lock (_startLock)
            {
                if (IsStarted)
                    throw new InvalidOperationException("Импортировать снапшот очереди можно только когда процесс сбора страниц остановлен или ещё не начат.");

                _workersSharedState.Queue = new ConcurrentMultiReaderQueue<IQueuedUrl>(Settings.NumberOfWorkers, snapshot, QueuedUrlEqualityComparer.Instance);
            }
        }

        /// <summary>
        /// Метод, экспортирующий снапшот текущего состояния очереди URL-адресов.
        /// </summary>
        /// <returns>Снапшот текущего состояния очереди URL-запросов.</returns>
        public IMultiReaderQueueSnapshot<IQueuedUrl> ExportQueueSnapshot()
        {
            return _workersSharedState.Queue.ExportQueueSnapshot();
        }

        /// <summary>
        /// Метод, добавляющий URL-адреса в очередь.
        /// </summary>
        /// <param name="urls">Один или несколько URL-адресов, доторые требуется добавить в очередь.</param>
        /// <exception cref="InvalidOperationException">Процесс сбора страниц уже запущен.</exception>
        public void AddUrls(params Uri[] urls)
        {
            lock (_startLock)
            {
                if (IsStarted)
                    throw new InvalidOperationException("Добавлять URL-адреса в очередь можно только когда процесс сбора страниц остановлен или ещё не начат.");

                _ = _workersSharedState.Queue.TryEnqueueMany(urls.Select(url => new QueuedUrl(url)));
            }
        }

        /// <summary>
        /// Метод, запускающий или возобновляющий процесс сбора страниц.
        /// </summary>
        [MemberNotNull(nameof(_workers))]
        public void Start()
        {
            lock (_startLock)
            {
                if (IsStarted)
                    throw new InvalidOperationException("Процесс сбора страниц уже запущен.");

                _workersSharedState.ResetState();

                // Два дополнительных воркера - для сброса количества сделанных запросов раз в секунду и периодического сохранения снапшотов очереди.
                _workers = new Task[Settings.NumberOfWorkers + 2];

                PeriodicTimer timer = new(TimeSpan.FromSeconds(1));
                _workers[0] = Task.Run(() => TimerActionAsync(timer, _workersSharedState), _workersSharedState.CancellationToken);
                _workers[1] = Task.Run(() => QueueSnapshotActionAsync(_workersSharedState), _workersSharedState.CancellationToken);

                for (int i = 0; i < Settings.NumberOfWorkers; i++)
                {
                    int workerId = i;
                    _workers[i + 2] = Task.Run(() => WorkerActionAsync(workerId, _workersSharedState), _workersSharedState.CancellationToken);
                }

                IsStarted = true;
            }
        }

        /// <summary>
        /// Метод, принудительно приостанавливающий процесс сбора страниц.
        /// </summary>
        public void Stop()
        {
            Stop(isExternalReason: true);
        }

        /// <summary>
        /// Метод, завершающий процесс сбора страниц и освобождающий используемые неуправляемые ресурсы.
        /// </summary>
        public void Dispose()
        {
            Stop();
            _workersSharedState.Dispose();
        }

        private void Stop(bool isExternalReason)
        {
            lock (_startLock)
            {
                if (!IsStarted)
                    return;

                _workersSharedState.RequestCancellation();
                Task.WaitAll(_workers);

                IsStarted = false;
            }

            // Если остановку запросили не извне - значит, очередь опустела и содержимое всех страниц собрано,
            // сообщим об этом всем интересующимся, кто подписался на событие AllPagesCollected.
            // Если остановку запросили извне, то предполагается, что все интересующиеся и так об этом знают.
            if (!isExternalReason)
                AllPagesCollected?.Invoke();
        }
    }
}
