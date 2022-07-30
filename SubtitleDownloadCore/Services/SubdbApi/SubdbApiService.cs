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

        public async Task<string> SearchSubtitleAsync(string movieFilePath)
        {
            WriteLine($"Searching subtitle for {Path.GetFileNameWithoutExtension(movieFilePath)} , wait...");

            using (HttpClient httpClient = new HttpClient())
            {
                string subdbApiFileHash = FileUtil.GetSubdbFileHash(movieFilePath);

                string urlSearch = $"http://api.thesubdb.com/?action=search&hash={subdbApiFileHash}";

                httpClient.DefaultRequestHeaders.Add("User-Agent", Constants.USER_AGENT);
                HttpResponseMessage searchResponse = await httpClient.GetAsync(urlSearch);

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
        }

        public async Task DownloadSubtitlesAsync(List<string> movieFiles)
        {
            foreach (var movieFilePath in movieFiles)
            {
                WriteLine(string.Empty);

                string srtFilePath = Path.Combine(Path.GetDirectoryName(movieFilePath), Path.GetFileNameWithoutExtension(movieFilePath)) + ".srt";
                if (File.Exists(srtFilePath))
                {
                    WriteLine($"Subtitles already downloaded for {Path.GetFileNameWithoutExtension(movieFilePath)}. Manually delete the .srt files to download again.");
                    continue;
                }

                string languagesFound = await SearchSubtitleAsync(movieFilePath);

                if (!string.IsNullOrWhiteSpace(languagesFound))
                {
                    await TryDownloadSubsAsync(srtFilePath, FileUtil.GetSubdbFileHash(movieFilePath), languagesFound);
                }
                else
                {
                    WriteLine($"Subtitles for languages '{Constants.LANGUAGE_EN}','{Constants.LANGUAGE_PT}' not found :( ");
                }

            }
        }


        private static async Task TryDownloadSubsAsync(string srtFilePath, string subdbApiFileHash, string languagesFound)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                WriteLine("Subtitle(s) found (languages = " + languagesFound + ") !");
                WriteLine($"Will download only '{Constants.LANGUAGE_EN}' and '{Constants.LANGUAGE_PT}' ...");

                if (languagesFound.Contains(Constants.LANGUAGE_EN))
                {
                    WriteLine($"Downloading '{Constants.LANGUAGE_EN}' ... ");
                    await DownloadSingleSubtitleAsync(subdbApiFileHash, srtFilePath, httpClient, Constants.LANGUAGE_EN);
                }

                if (languagesFound.Contains(Constants.LANGUAGE_PT))
                {
                    WriteLine($"Downloading '{Constants.LANGUAGE_PT}' ... ");
                    await DownloadSingleSubtitleAsync(subdbApiFileHash, srtFilePath, httpClient, Constants.LANGUAGE_PT);
                }
            }
        }



        private static async Task DownloadSingleSubtitleAsync(string subdbApiFileHash, string srtFilePath, HttpClient httpClient, string language)
        {
            string urlDownload = $"http://api.thesubdb.com/?action=download&hash={subdbApiFileHash}&language=" + language;
            HttpResponseMessage downloadResponse = await httpClient.GetAsync(urlDownload);

            if (downloadResponse.IsSuccessStatusCode)
            {
                HttpContent httpContent = downloadResponse.Content;
                var subtitleFilePath = srtFilePath;

                if (language.Equals(Constants.LANGUAGE_PT) && File.Exists(subtitleFilePath))
                {
                    subtitleFilePath = subtitleFilePath.Replace(".srt", "-pt.srt");
                }

                await FileUtil.WriteHttpContentToFileAsync(httpContent, subtitleFilePath);

                WriteLine("Subtitle downloaded -> " + subtitleFilePath);
            }
            else
            {
                WriteLine("Failed to download. HTTP Status = " + downloadResponse.StatusCode);
            }
        }

    }
}
