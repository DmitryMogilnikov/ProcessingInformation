using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core.Resolvers;

namespace WebCrawler.Core.UnitTests.Resolvers
{
    public class UrlResolverTests
    {

        [Fact]
        public void TestTryResolveUrl()
        {
            UrlResolver resolver = new UrlResolver();
            Uri url = new Uri("https://test.net/page1");

            Uri resolvedUrl;
            bool act = resolver.TryResolveUrl("page2", url, out resolvedUrl);

            Assert.True(act);
            Assert.Equal("https://test.net/page1/page2/", resolvedUrl.ToString());
        }

        [Fact]
        public void TestTryResolveUrl_WithRelativeBaseUrl()
        {
            UrlResolver resolver = new UrlResolver();
            Uri url = new Uri("some/page.html", UriKind.Relative);

            Uri resolvedUrl;
            bool act = resolver.TryResolveUrl("", url, out resolvedUrl);

            Assert.False(act);
        }

        [Fact]
        public void TestTryResolveUrl_WithFileUrl()
        {
            UrlResolver resolver = new UrlResolver();
            Uri url = new Uri("file:///folder/page1.html");

            Uri resolvedUrl;
            bool act = resolver.TryResolveUrl("page2.html", url, out resolvedUrl);

            Assert.True(act);
            Assert.Equal("file://folder/page2.html/", resolvedUrl.ToString());
        }
    }
}
