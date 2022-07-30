using FluentAssertions;
using SubtitleDownloadCore.Services.SubdbApi;
using System.IO;
using Xunit;

namespace SubtitleDownloadCore.Tests
{
    public class SubdbFileHashTests
    {

        [Fact]
        public void SubdbFileHash_Should_Be_Correct()
        {
            var testFilePath = Path.GetTempFileName();
            string testData = new string('a', 100000);
            File.WriteAllText(testFilePath, testData);

            const string EXPECTED_HASH = "81615449a98aaaad8dc179b3bec87f38";

            string hash = SubdbApiService.GetSubdbFileHash(testFilePath);

            hash.Should().Be(EXPECTED_HASH);

            File.Delete(testFilePath);
        }

    }
}
