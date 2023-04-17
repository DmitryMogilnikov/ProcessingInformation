using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebCrawler.Core.Factories;
using WebCrawler.Core.Getters;
using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;
using WebCrawler.Core.Models;
using WebCrawler.Core.Resolvers;
using WebCrawler.Core.Schedulers;

namespace WebCrawler.CLI
{
    internal class Program
    {
        static async Task Main()
        {
            // TODO: Добавить возможность нормально настраивать сбор - через аргументы командной строки или интерактивно.
            ConcurrentStatistics statistics = new();
            HttpDataGetterFactory httpDataGetterFactory = new();
            CreatingFactory<FileDataGetter> fileDataGetterFactory = new();
            SubdomainOnlyUrlResolver urlResolver = new(statistics);
            HtmlContentParserFactory contentParserFactory = new(urlResolver, statistics);
            string timestampString = DateTime.UtcNow.ToString("yyyy-MM-ddTHH_mm_ss");
            string outputFilePath = $"./test_output/content_{timestampString}.csv";
            ConcurrentCsvContentSaverCachingFactory contentSaverFactory = new(outputFilePath);
            TimestampFilePathTemplate filePathTemplate = new("./test_output/", "QueueSnapshot_{0}.snp");
            FileMultiReaderQueueSnapshotSaverFactory<IQueuedUrl> snapshotSaverFactory = new(filePathTemplate);

            SchedulerSettings schedulerSettings = new(NumberOfWorkers: 100,
                                                      RequestsPerSecond: 1000,
                                                      MaxRetries: 2,
                                                      QueueSnapshotSavePeriod: SchedulerSettings.DefaultQueueSnapshotSavePeriod,
                                                      new IFactory<IDataGetter>[] { httpDataGetterFactory, fileDataGetterFactory },
                                                      new[] { contentParserFactory },
                                                      ContentSaverFactory: contentSaverFactory,
                                                      QueueSnapshotSaverFactory: snapshotSaverFactory,
                                                      statistics);
            using Scheduler scheduler = new(schedulerSettings);

            string currentDirectory = Path.TrimEndingDirectorySeparator(Directory.GetCurrentDirectory());
            // Дерево страниц для университетского сайта очень большое, это займёт кучу времени!
            //scheduler.AddUrls(new Uri("https://spbu.ru"));
            scheduler.AddUrls(new Uri("https://msu.ru"));

            DateTime startTime = default;
            bool isFinished = false;
            scheduler.AllPagesCollected += () =>
            {
                TimeSpan sessionDuration = DateTime.Now - startTime;

                IMultiReaderQueueSnapshot<IQueuedUrl> snapshot = scheduler.ExportQueueSnapshot();
                long downloadedCount = CountLines(outputFilePath);
                Console.WriteLine($"Все страницы загружены. Всего посещено {snapshot.AlreadyProcessedEntries.Count} страниц, загружено {downloadedCount} страниц. Текущий сеанс длился {sessionDuration}.");
                isFinished = true;
            };
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;

                scheduler.Stop();

                TimeSpan sessionDuration = DateTime.Now - startTime;
                IMultiReaderQueueSnapshot<IQueuedUrl> snapshot = scheduler.ExportQueueSnapshot();
                snapshotSaverFactory.Create().TrySaveSnapshot(snapshot);

                int queueSize = snapshot.Buckets.Aggregate(0, (acc, curr) => acc + curr?.Length ?? 0);
                int visitedCount = snapshot.AlreadyProcessedEntries.Count - queueSize;
                long downloadedCount = CountLines(outputFilePath);
                Console.WriteLine($"Досрочное прерывание. В очереди {queueSize} адресов, посещено {visitedCount} страниц, загружено {downloadedCount} страниц. Текущий сеанс длился {sessionDuration}.");
                isFinished = true;
            };

            startTime = DateTime.Now;
            scheduler.Start();
            while (!isFinished)
                await Task.Delay(TimeSpan.FromSeconds(15));

            WriteStatistics(statistics);
            Console.ReadKey();
        }

        private static long CountLines(string fileName)
        {
            long result = 0;
            try
            {
                using StreamReader reader = new(fileName);
                while (reader.ReadLine() is not null)
                    result++;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return result;
        }

        private static void WriteStatistics(IStatistics statistics)
        {
            Console.WriteLine("Статистика сбора:");
            Console.WriteLine(statistics.ToString());
        }
    }
}