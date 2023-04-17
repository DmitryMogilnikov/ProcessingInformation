using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core.Comparers;
using WebCrawler.Core.Models;

namespace WebCrawler.Core.UnitTests.Comparers
{
    public class QueuedUrlEqualityComparerTests
    {
        [Fact]
        public void TestEquals()
        {
            QueuedUrlEqualityComparer comparer = QueuedUrlEqualityComparer.Instance;
            QueuedUrl firstUrl = new QueuedUrl(new Uri("https://localhost"), 0);
            QueuedUrl secondUrl = new QueuedUrl(new Uri("https://localhost"), 1);

            bool act = comparer.Equals(firstUrl, secondUrl);

            Assert.True(act);
        }
    }
}
