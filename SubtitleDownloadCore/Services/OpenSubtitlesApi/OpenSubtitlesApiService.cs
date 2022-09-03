using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SubtitleDownloadCore.Services.OpenSubtitlesApi
{

    /// <summary>
    /// https://opensubtitles.stoplight.io/docs/opensubtitles-api/b1eb44d4c8502-open-subtitles-api
    /// </summary>
    public class OpenSubtitlesApiService : ISubtitleService
    {
        private const string BASE_API_URL = "https://api.opensubtitles.com/api/v1/";

        private readonly HttpClient _httpClient;

        public OpenSubtitlesApiService()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Api-Key", config["OpenSubtitlesApiKey"]);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", Program.USER_AGENT);
        }


        public async Task<IList<string>> DownloadSubtitlesAsync(string movieFilePath, string srtDownloadPath)
        {
            var movieFileHash = OpenSubtitlesFileHasher.ToHexadecimal(OpenSubtitlesFileHasher.ComputeMovieHash(movieFilePath));

            OpenSubtitlesSearchResponseDto openSubtitlesSearchResponseDto = await SearchSubtitleAsync(movieFileHash);

            if (openSubtitlesSearchResponseDto.TotalCount > 0)
            {
                var srtFilesFound = openSubtitlesSearchResponseDto.Data
                    .Where(i => i.Attributes.Language.Equals(Program.LANGUAGE_EN) || i.Attributes.Language.Equals(Program.LANGUAGE_PT))
                    .SelectMany(i => i.Attributes.Files);

                return await TryDownloadSrtFilesAsync(srtFilesFound, srtDownloadPath);
            }

            return new List<string>();
        }


        private async Task<OpenSubtitlesSearchResponseDto> SearchSubtitleAsync(string movieFileHash)
        {
            var searchUrl = BASE_API_URL + "subtitles?moviehash=" + movieFileHash;
            var response = await _httpClient.GetAsync(searchUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OpenSubtitlesSearchResponseDto>(json);
            }

            throw new SubtitleServiceException($"Error searching via OpenSubtitles API (http {(int)response.StatusCode})");
        }


        private async Task<IList<string>> TryDownloadSrtFilesAsync(IEnumerable<File> srtFilesFound, string srtDownloadPath)
        {
            IList<string> subs = new List<string>();

            foreach (var srtFileFound in srtFilesFound)
            {
                subs.Add(await DownloadSingleSubtitleAsync(srtFileFound.FileId, srtDownloadPath));
            }

            return subs;
        }

        private async Task<string> DownloadSingleSubtitleAsync(int fileId, string srtDownloadPath)
        {
            var downloadUrl = BASE_API_URL + "download";

            OpenSubtitlesDownloadRequestDto openSubtitlesDownloadRequestDto = new OpenSubtitlesDownloadRequestDto()
            {
                FileId = fileId
            };

            var content = new StringContent(JsonSerializer.Serialize(openSubtitlesDownloadRequestDto), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(downloadUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                OpenSubtitlesDownloadResponseDto openSubtitlesDownloadResponseDto = JsonSerializer.Deserialize<OpenSubtitlesDownloadResponseDto>(json);

                using var srtStream = _httpClient.GetStreamAsync(openSubtitlesDownloadResponseDto.Link);
                using var srtFileStream = new FileStream(srtDownloadPath, FileMode.OpenOrCreate);
                srtStream.Result.CopyTo(srtFileStream);
            }

            throw new SubtitleServiceException($"Error downloading via OpenSubtitles API (http {(int)response.StatusCode})");
        }


    }
}