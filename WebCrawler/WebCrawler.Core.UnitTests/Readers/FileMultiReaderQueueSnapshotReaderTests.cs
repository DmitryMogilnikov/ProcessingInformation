using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core.Collections;
using WebCrawler.Core.Models;
using WebCrawler.Core.Readers;

namespace WebCrawler.Core.UnitTests.Readers
{
    public class FileMultiReaderQueueSnapshotReaderTests
    {
        [Fact]
        public void TSnapshotReadSnapshot()
        {
            FileMultiReaderQueueSnapshotReader<string, MultiReaderQueueSnapshot<string>> concurrentQueue = new FileMultiReaderQueueSnapshotReader<string, MultiReaderQueueSnapshot<string>>();

            string source = "path_file";

            Action act = () => concurrentQueue.ReadSnapshot(source);


            Assert.Throws<FileNotFoundException>(act);
        }


    }
}
