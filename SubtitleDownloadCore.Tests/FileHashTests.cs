using System;
using System.IO;
using Xunit;

namespace SubtitleDownloadCore.Tests
{
    public sealed class FileHashTests : IDisposable
    {

        private readonly string TestFilePath;

        public FileHashTests()
        {
            TestFilePath = Path.GetTempFileName();
            string testData = new string('a', 100000);
            File.WriteAllText(TestFilePath, testData);
        }


        public void Dispose()
        {
            File.Delete(TestFilePath);            
        }


        [Fact]
        public void CorrectFileHashTest()
        {
            string correctHash = "81615449a98aaaad8dc179b3bec87f38";

            string hash = FileHashUtil.GetSubdbFileHash(TestFilePath);

            Assert.Equal(hash, correctHash);
        }

    }
}
