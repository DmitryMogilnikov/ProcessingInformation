using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core.Getters;

namespace WebCrawler.Core.UnitTests.Getters
{
    public class FileDataGetterTests
    {
        [Fact]
        public void TestCanGetContent_WithFileUrl()
        {
            FileDataGetter dataGetter = new FileDataGetter();

            bool act = dataGetter.CanGetContent(new Uri("file:///some/path/to/file"));

            Assert.True(act);
        }

        [Fact]
        public void TestCanGetContent_WithNonFileUrl()
        {
            FileDataGetter dataGetter = new FileDataGetter();

            bool act = dataGetter.CanGetContent(new Uri("http://localhost"));

            Assert.False(act);
        }
    }
}
