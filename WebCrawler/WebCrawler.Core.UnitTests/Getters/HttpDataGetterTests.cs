using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core.Getters;

namespace WebCrawler.Core.UnitTests.Getters
{
    public class HttpDataGetterTests
    {
        [Fact]
        public void TestCanGetContent_WithHttpUrl()
        {
            HttpDataGetter dataGetter = new HttpDataGetter(null);

            bool act = dataGetter.CanGetContent(new Uri("http://localhost"));

            Assert.True(act);
        }

        [Fact]
        public void TestCanGetContent_WithNonHttpUrl()
        {
            HttpDataGetter dataGetter = new HttpDataGetter(null);

            bool act = dataGetter.CanGetContent(new Uri("ftp://filestorage"));

            Assert.False(act);
        }
    }
}
