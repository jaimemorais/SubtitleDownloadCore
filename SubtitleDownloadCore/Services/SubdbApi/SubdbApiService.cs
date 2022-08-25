using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Console;

namespace SubtitleDownloadCore.Services.SubdbApi
{

    /// <summary>
    /// http://thesubdb.com/api/ 
    /// </summary>
    public class SubdbApiService : ISubtitleService
    {
        private readonly HttpClient _httpClient;

        public SubdbApiService()
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("User-Agent", Program.USER_AGENT);
        }

        public async Task<IList<string>> DownloadSubtitlesAsync(string movieFilePath, string srtFilePath)
        {
            string languagesFound = await SearchSubtitleAsync(movieFilePath);

            if (!string.IsNullOrWhiteSpace(languagesFound))
            {
                return await TryDownloadSubsAsync(srtFilePath, SubDbFileUtils.GetSubdbFileHash(movieFilePath), languagesFound);
            }

            return new List<string>();
        }

        private async Task<string> SearchSubtitleAsync(string movieFilePath)
        {
            WriteLine($"Searching subtitle for {Path.GetFileNameWithoutExtension(movieFilePath)} , wait...");

            string subdbApiFileHash = SubDbFileUtils.GetSubdbFileHash(movieFilePath);

            string urlSearch = $"http://api.thesubdb.com/?action=search&hash={subdbApiFileHash}";

            HttpResponseMessage searchResponse = await _httpClient.GetAsync(urlSearch);

            if (searchResponse.IsSuccessStatusCode)
            {
                return await searchResponse.Content.ReadAsStringAsync();
            }
            else
            {
                WriteLine($"Search failed. HTTP Status = {searchResponse.StatusCode}");
                return null;
            }
        }



        private async Task<IList<string>> TryDownloadSubsAsync(string srtFilePath, string subdbApiFileHash, string languagesFound)
        {
            IList<string> subs = new List<string>();

            if (languagesFound.Contains(Program.LANGUAGE_EN))
            {
                WriteLine($"Downloading '{Program.LANGUAGE_EN}' ... ");
                subs.Add(await DownloadSingleSubtitleAsync(subdbApiFileHash, srtFilePath, Program.LANGUAGE_EN));
            }

            if (languagesFound.Contains(Program.LANGUAGE_PT))
            {
                WriteLine($"Downloading '{Program.LANGUAGE_PT}' ... ");
                subs.Add(await DownloadSingleSubtitleAsync(subdbApiFileHash, srtFilePath, Program.LANGUAGE_PT));
            }

            return subs;
        }



        private async Task<string> DownloadSingleSubtitleAsync(string subdbApiFileHash, string srtFilePath, string language)
        {
            string urlDownload = $"http://api.thesubdb.com/?action=download&hash={subdbApiFileHash}&language=" + language;
            HttpResponseMessage downloadResponse = await _httpClient.GetAsync(urlDownload);

            if (downloadResponse.IsSuccessStatusCode)
            {
                HttpContent httpContent = downloadResponse.Content;
                var subtitleFilePath = srtFilePath;

                if (language.Equals(Program.LANGUAGE_PT) && File.Exists(subtitleFilePath))
                {
                    subtitleFilePath = subtitleFilePath.Replace(".srt", "-pt.srt");
                }

                await SubDbFileUtils.WriteHttpContentToFileAsync(httpContent, subtitleFilePath);

                return subtitleFilePath;
            }
            else
            {
                throw new SubtitleServiceException("Failed to download. HTTP Status = " + downloadResponse.StatusCode);
            }
        }




    }
}
