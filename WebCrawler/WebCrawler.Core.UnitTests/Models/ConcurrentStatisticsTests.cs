using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core.Collections;
using WebCrawler.Core.Models;

namespace WebCrawler.Core.UnitTests.Models
{
    public class ConcurrentStatisticsTests
    {
        [Fact]
        public void TestIncrementProcessedPagesCount()
        {

            ConcurrentStatistics stat = new ConcurrentStatistics();

            ulong initialvalue = stat.ProcessedPagesCount;

            stat.IncrementProcessedPagesCount();

            Assert.Equal(initialvalue + 1, stat.ProcessedPagesCount);

        }


        [Fact]
        public void TestAddToUniqueInternalLinksCount()
        {

            ConcurrentStatistics stat = new ConcurrentStatistics();

            ulong init_v = stat.UniqueInternalLinksCount;

            stat.AddToUniqueInternalLinksCount(5);

            Assert.Equal(init_v + 5, stat.UniqueInternalLinksCount);

        }


        [Fact]
        public void TestIncrementBrokenLinksCount()
        {

            ConcurrentStatistics stat = new ConcurrentStatistics();

            ulong init = stat.BrokenLinksCount;

            stat.IncrementBrokenLinksCount();

            Assert.Equal(init + 1, stat.BrokenLinksCount);

        }


        [Fact]
        public void TeAddInternalSubdomen()
        {

            ConcurrentStatistics stat = new ConcurrentStatistics();

            string subdomen = "str";
      

            IReadOnlyCollection<string> orig = stat.UniqueInternalSubdomens;

            stat.AddInternalSubdomen(subdomen);

            Assert.Contains(subdomen, stat.UniqueInternalSubdomens);
                
                
                
               

        }

    }
    
}



