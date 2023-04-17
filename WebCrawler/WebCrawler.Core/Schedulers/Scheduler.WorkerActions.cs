using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;
using WebCrawler.Core.Models;

namespace WebCrawler.Core.Schedulers
{
    // В этой части класса описаны методы, работающие в воркер-потоках.
    public partial class Scheduler
    {
        private static readonly TimeSpan WorkersDelay = TimeSpan.FromMilliseconds(10);

        private static async Task WorkerActionAsync(int workerId, WorkersSharedState sharedState)
        {
            CancellationToken cancellationToken = sharedState.CancellationToken;
            IMultiReaderQueue<IQueuedUrl> queue = sharedState.Queue;
            ISchedulerSettings settings = sharedState.Settings;

            IEnumerable<IDataGetter> dataGetters = settings.DataGetterFactories.Select(factory => factory.Create());
            IEnumerable<IContentParser> contentParsers = settings.ContentParserFactories.Select(factory => factory.Create());
            using IContentSaver contentSaver = settings.ContentSaverFactory.Create();

            IStatistics statistics = settings.Statistics;

            bool isIdle = false;

            while (!cancellationToken.IsCancellationRequested)
            {
                // Пытаемся увеличить число запросов, сделанных за эту секунду.
                // Если не получается - значит, лимит исчерпан, ждём и пробуем ещё раз.
                while (!sharedState.TryIncrementRequestsCount())
                {
                    await Task.Delay(WorkersDelay);
                    if (cancellationToken.IsCancellationRequested)
                        return;
                }

                // Если для воркера будет работа, и он до этого простаивал - выводим его из состояния простоя и сообщаем,
                // что количество простаивающих воркеров уменьшилось на 1. Мы делаем это ДО того, как забираем элемент из очереди,
                // т.к. в противном случае может быть промежуток времени, когда воркер ещё считается простаивающим,
                // в очереди уже нет элемента, а соответствующая страница ещё не обработана.
                if (!queue.IsBucketEmpty(workerId) && isIdle)
                {
                    isIdle = false;
                    sharedState.DecrementIdleWorkersCount();
                }

                // Пытаемся получить следующий элемент очереди для текущего воркера.
                // Если не получилось - не думаем, что работа закончилась, возможно,
                // остальные воркеры ещё положат в очередь элементы, которые этому надо будет обрабатывать. 
                if (!queue.TryDequeue(workerId, out IQueuedUrl? queuedUrl))
                {
                    // Если воркер ещё не в состоянии простоя - переводим его в него и сообщаем, что количество простаивающих воркеров увеличилось на 1.
                    if (!isIdle)
                    {
                        isIdle = true;
                        sharedState.IncrementIdleWorkersCount();
                    }

                    // Если все страницы обработаны - завершаем работу этого воркера и сообщаем, что количество завершивших работу воркеров увеличилось на 1.
                    if (sharedState.IsAllWorkDone())
                    {
                        sharedState.IncrementFinishedWorkersCount();
                        return;
                    }

                    // Если кто-то ещё работает или в очереди есть данные - ждём и снова проверяем, нет ли для этого воркера работы.
                    await Task.Delay(WorkersDelay);
                    continue;
                }

                Uri url = queuedUrl.Url;
                DateTime timestamp = DateTime.UtcNow;

                string? pageData = null;
                foreach (var dataGetter in dataGetters)
                {
                    if (dataGetter.CanGetContent(url))
                    {
                        pageData = await dataGetter.GetContentAsync(url);
                        if (pageData is not null)
                            break;
                    }
                }

                IPageContent? pageContent = null;
                if (pageData is not null)
                {
                    foreach (var contentParser in contentParsers)
                    {
                        pageContent = await contentParser.ParseContentAsync(pageData, url);
                        if (pageContent is not null) 
                            break;
                    }
                }

                if (pageContent is null)
                {
                    // Если по какой-то причине не получилось загрузить содержимое страницы -
                    // проверяем, что мы её ещё не слишком много раз пробовали загрузить, и если нет - возвращаем в очередь.
                    if (queuedUrl.RequestsCount < sharedState.Settings.MaxRetries)
                        sharedState.Queue.TryEnqueue(new QueuedUrl(url, queuedUrl.RequestsCount + 1));
                    else
                        statistics.IncrementBrokenLinksCount();
                    continue;
                }

                IReadOnlyCollection<IQueuedUrl> linksFailedToEnqueue = sharedState.Queue.TryEnqueueMany(pageContent.Links.Select(link => new QueuedUrl(link)));
                _ = contentSaver.TrySaveContent(url, timestamp, pageContent.TextContent);

                statistics.IncrementProcessedPagesCount();
                int totalLinks = pageContent.Links.Count();
                int uniqueLinks = totalLinks - linksFailedToEnqueue.Count;
                statistics.AddToUniqueInternalLinksCount((ulong)uniqueLinks);

                Debug.WriteLine($"Обработано {statistics.ProcessedPagesCount} страниц из {statistics.UniqueInternalLinksCount - statistics.BrokenLinksCount}.");
            }
        }

        private static async Task TimerActionAsync(PeriodicTimer timer, WorkersSharedState sharedState)
        {
            while (!sharedState.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    await timer.WaitForNextTickAsync(sharedState.CancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                sharedState.ResetRequestsCount();
            }
        }

        private static async Task QueueSnapshotActionAsync(WorkersSharedState sharedState)
        {
            ISchedulerSettings settings = sharedState.Settings;
            IMultiReaderQueue<IQueuedUrl> queue = sharedState.Queue;

            PeriodicTimer timer = new(settings.QueueSnapshotSavePeriod);
            IMultiReaderQueueSnapshotSaver<IQueuedUrl> queueSnapshotSaver = settings.QueueSnapshotSaverFactory.Create();

            while (!sharedState.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    await timer.WaitForNextTickAsync(sharedState.CancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                IMultiReaderQueueSnapshot<IQueuedUrl> queueSnapshot = queue.ExportQueueSnapshot();
                _ = queueSnapshotSaver.TrySaveSnapshot(queueSnapshot);
            }
        }
    }
}
