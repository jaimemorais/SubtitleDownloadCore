using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
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
            _httpClient.DefaultRequestHeaders.Add("User-Agent", Constants.USER_AGENT);
        }


        public async Task<IList<string>> DownloadSubtitlesAsync(string movieFilePath, string srtFilePath)
        {
            var movieFileHash = OpenSubtitlesFileHasher.ToHexadecimal(OpenSubtitlesFileHasher.ComputeMovieHash(movieFilePath));

            OpenSubtitlesSearchResponseDto openSubtitlesSearchResponseDto = await SearchSubtitleAsync(movieFileHash);

            if (openSubtitlesSearchResponseDto.TotalCount > 0)
            {
                return await TryDownloadSubsAsync(srtFilePath, movieFileHash, openSubtitlesSearchResponseDto);
            }

            return new List<string>();
        }


        private async Task<OpenSubtitlesSearchResponseDto> SearchSubtitleAsync(string movieFileHash)
        {
            var searchUrl = BASE_API_URL + SUBTITLES_ENDPOINT + "?moviehash=" + movieFileHash;
            var response = await _httpClient.GetAsync(searchUrl);

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<OpenSubtitlesSearchResponseDto>(await response.Content.ReadAsStringAsync());
            }

            throw new SubtitleServiceException($"Error searching via OpenSubtitles API (http {(int)response.StatusCode})");
        }


        private async Task<IList<string>> TryDownloadSubsAsync(string srtFilePath, string movieFileHash, OpenSubtitlesSearchResponseDto openSubtitlesSearchResponseDto)
        {
            IList<string> subs = new List<string>();

            foreach (var item in openSubtitlesSearchResponseDto.Data)
            {
                var language = item.Attributes.Language;
                if (language.Equals(Constants.LANGUAGE_PT) || language.Equals(Constants.LANGUAGE_EN))
                {
                    subs.Add(await DownloadSingleSubtitleAsync(item.Attributes.Url, srtFilePath, language));
                }
            }

            return subs;
        }

        private static async Task<string> DownloadSingleSubtitleAsync(string subUrl, string srtFilePath, string language)
        {
            // TODO 

            return null;
        }


    }
}