using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly string _apiKey;
        private const string USER_AGENT = "SubtitleDownloadCore";

        private const string BASE_API_URL = "https://api.opensubtitles.com/api/v1/";
        private const string SUBTITLES_ENDPOINT = "subtitles";


        public OpenSubtitlesApiService()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

            _apiKey = config["OpenSubtitlesApiKey"];
        }

        public async Task<string> SearchSubtitleAsync(string movieFilePath)
        {
            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Api-Key", _apiKey);
            httpClient.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);

            var searchUrl = GetSearchUrl(movieFilePath);
            var responseHash = await httpClient.GetAsync(searchUrl);


            var responseHashBody = await responseHash.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<IList<OpenSubtitlesItem>>(responseHashBody).ToList();


            throw new NotImplementedException();
        }

        public async Task DownloadSubtitlesAsync(List<string> movieFiles)
        {

            foreach (string movieFile in movieFiles)
            {
                await SearchSubtitleAsync(movieFile);
            }

        }


        private string GetSearchUrl(string movieFilePath)
        {
            var moviehash = OpenSubtitlesFileHasher.ToHexadecimal(OpenSubtitlesFileHasher.ComputeMovieFileHash(movieFilePath));
            var file = new FileInfo(movieFilePath);
            var movieByteSize = file.Length;

            if (movieByteSize <= 0)
            {
                throw new Exception("Selected file is empty.");
            }

            return BASE_API_URL + SUBTITLES_ENDPOINT + "/moviehash=" + moviehash;
        }

    }
}
