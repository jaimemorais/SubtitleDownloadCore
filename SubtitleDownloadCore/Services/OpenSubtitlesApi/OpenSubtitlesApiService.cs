using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Console;

namespace SubtitleDownloadCore.Services.OpenSubtitlesApi
{

    /// <summary>
    /// https://opensubtitles.stoplight.io/docs/opensubtitles-api/b1eb44d4c8502-open-subtitles-api
    /// </summary>
    public class OpenSubtitlesApiService : ISubtitleService
    {
        private const string USER_AGENT = "SubtitleDownloadCore";

        private const string BASE_API_URL = "https://api.opensubtitles.com/api/v1/";
        private const string SUBTITLES_ENDPOINT = "subtitles";

        private readonly HttpClient _httpClient;

        public OpenSubtitlesApiService()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Api-Key", config["OpenSubtitlesApiKey"]);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
        }


        public async Task DownloadSubtitlesAsync(string movieFilePath, string srtFilePath)
        {
            var movieFileHash = OpenSubtitlesFileHasher.ToHexadecimal(OpenSubtitlesFileHasher.ComputeMovieHash(movieFilePath));

            OpenSubtitlesSearchResponseDto openSubtitlesSearchResponseDto = await SearchSubtitleAsync(movieFileHash);

            if (openSubtitlesSearchResponseDto.TotalCount > 0)
            {
                await TryDownloadSubsAsync(srtFilePath, movieFileHash, openSubtitlesSearchResponseDto);
            }
            else
            {
                WriteLine($"Subtitles for languages '{Constants.LANGUAGE_EN}','{Constants.LANGUAGE_PT}' not found :( ");
            }
        }


        private async Task<OpenSubtitlesSearchResponseDto> SearchSubtitleAsync(string movieFileHash)
        {
            var searchUrl = BASE_API_URL + SUBTITLES_ENDPOINT + "?moviehash=" + movieFileHash;
            var response = await _httpClient.GetAsync(searchUrl);

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<OpenSubtitlesSearchResponseDto>(await response.Content.ReadAsStringAsync());
            }

            throw new SubtitleServiceException($"Error searching via OpenSubtitles (http {(int)response.StatusCode})");
        }


        private Task TryDownloadSubsAsync(string srtFilePath, string movieFileHash, OpenSubtitlesSearchResponseDto openSubtitlesSearchResponseDto)
        {
            throw new NotImplementedException();
        }

    }
}
