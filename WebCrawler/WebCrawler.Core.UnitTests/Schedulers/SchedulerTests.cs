using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core.Factories;
using WebCrawler.Core.Interfaces;
using WebCrawler.Core.Interfaces.Models;
using WebCrawler.Core.Models;
using WebCrawler.Core.Schedulers;

namespace WebCrawler.Core.UnitTests.Schedulers
{
    public class SchedulerTests
    {
        private class MultiReaderQueueSnapshotSaverMock : IMultiReaderQueueSnapshotSaver<IQueuedUrl>
        {
            public bool TrySaveSnapshot(IMultiReaderQueueSnapshot<IQueuedUrl> snapshot)
            {
                return true;
            }
        }

        private class ContentSaverMock : IContentSaver
        {
            public bool TrySaveContent(Uri url, DateTime timestamp, string content)
            {
                return true;
            }

            public void Dispose()
            {
                return;
            }
        }

        [Fact]
        public void TestImportQueueSnapshot()
        {
            IFactory<IMultiReaderQueueSnapshotSaver<IQueuedUrl>> snapshotSaverFactory = new CreatingFactory<MultiReaderQueueSnapshotSaverMock>();
            IEnumerable<IFactory<IDataGetter>> dataGetterFactories = Enumerable.Empty<IFactory<IDataGetter>>();
            IEnumerable<IFactory<IContentParser>> contentParserFactories = Enumerable.Empty<IFactory<IContentParser>>();
            IFactory<IContentSaver> contentSaverFactory = new CreatingFactory<ContentSaverMock>();
            SchedulerSettings schedulerSettings = new SchedulerSettings(1, 0, 0, TimeSpan.FromDays(5), dataGetterFactories, contentParserFactories, contentSaverFactory, snapshotSaverFactory, null);
            Scheduler scheduler = new Scheduler(schedulerSettings);

            try
            {
                scheduler.Start();
                Action act = () => scheduler.ImportQueueSnapshot(null);

                Assert.Throws<InvalidOperationException>(act);
            }
            finally
            {
                scheduler.Stop();
            }
        }
    }
}
