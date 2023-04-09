using System;
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

            IWebDownloader webDownloader = settings.WebDownloaderFactory.Create();
            IContentSaver contentSaver = settings.ContentSaverFactory.Create();

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

                    // Если все воркеры в данный момент простаивают - это может значить, что работа действительно закончилась, но не обязательно -
                    // возможно, какой-то воркер успел подложить этому работы и уйти в простой уже после того, как мы проверили, что для этого воркера работы нет.
                    // Чтобы исключить такую ситуацию, дополнительно проверяем, что не только все воркеры простаивают, но и очередь пуста.
                    // Мы проверяем пустоту очереди только в тех случаях, когда все воркеры простаивают, потому что для ConcurrentMultiReaderQueue эта операция полностью блокирующая,
                    // а значит - дорогая, т.к. заставит ждать всех остальных воркеров. Но если они и так (предположительно) ничего не делают - то можно.
                    // Если очередь действительно пуста - завершаем работу этого воркера и сообщаем, что количество завершивших работу воркеров увеличилось на 1.
                    if (sharedState.IsAllWorkersIdle() && queue.IsEmpty())
                    {
                        sharedState.IncrementFinishedWorkersCount();
                        return;
                    }

                    // Если кто-то ещё работает или в очереди есть данные - ждём и снова проверяем, нет ли для этого воркера работы.
                    await Task.Delay(WorkersDelay);
                    continue;
                }

                // Если для воркера нашлась работа, и он до этого простаивал - выводим его из состояния простоя и сообщаем,
                // что количество простаивающих воркеров уменьшилось на 1.
                if (isIdle)
                {
                    isIdle = false;
                    sharedState.DecrementIdleWorkersCount();
                }

                Uri url = queuedUrl.Url;
                if (!webDownloader.TryGetPageContent(url, out IPageContent pageContent))
                {
                    // Если по какой-то причине не получилось загрузить содержимое страницы -
                    // проверяем, что мы её ещё не слишком много раз пробовали загрузить, и если нет - возвращаем в очередь.
                    if (queuedUrl.RequestsCount < sharedState.Settings.MaxRetries)
                        sharedState.Queue.TryEnqueue(new QueuedUrl(url, queuedUrl.RequestsCount + 1));
                    continue;
                }

                sharedState.Queue.TryEnqueueMany(pageContent.Links.Select(link => new QueuedUrl(link)));
                _ = contentSaver.TrySaveContent(url, pageContent.TextContent);
            }
        }

        private static async Task TimerActionAsync(PeriodicTimer timer, WorkersSharedState sharedState)
        {
            while (!sharedState.CancellationToken.IsCancellationRequested)
            {
                await timer.WaitForNextTickAsync();
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
                await timer.WaitForNextTickAsync();
                IMultiReaderQueueSnapshot<IQueuedUrl> queueSnapshot = queue.ExportQueueSnapshot();
                _ = queueSnapshotSaver.TrySaveSnapshot(queueSnapshot);
            }
        }
    }
}
