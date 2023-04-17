using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core.Models;

namespace WebCrawler.Core.UnitTests.Models
{
    public class CyclicNumberFilePathTemplateTests
    {
        [Fact]
        public void TestGetNextFilePath()
        {
            CyclicNumberFilePathTemplate filePathTemplate = new CyclicNumberFilePathTemplate("./test_folder/", "test_file_path_{0}.txt", 5);

            string filePath1 = filePathTemplate.GetNextFilePath();
            string filePath2 = filePathTemplate.GetNextFilePath();

            Assert.Equal("./test_folder/test_file_path_0.txt", filePath1);
            Assert.Equal("./test_folder/test_file_path_1.txt", filePath2);
        }
    }
}
