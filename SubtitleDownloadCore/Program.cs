using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SubtitleDownloadCore
{

    /// <summary>
    /// Subtitle Downloader 
    /// Using http://thesubdb.com/api/ 
    /// </summary>
    public class Program
    {
        const string LANGUAGE_EN = "en";
        const string LANGUAGE_PT = "pt";

        public static async Task Main(string[] args)
        {
            string customDir = (args.Any() && !string.IsNullOrEmpty(args[0])) ? args[0] : null;
            string rootDir = customDir ?? System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            Console.WriteLine(string.Empty);
            Console.WriteLine("SubtitleDownloadCore starting...");
            Console.WriteLine(string.Empty);


            var movieFiles = GetMovieFiles(rootDir);

            if (!movieFiles.Any())
            {
                Console.WriteLine("No movie files found.");
            }
            else
            {
                foreach (var movieFilePath in movieFiles)
                {
                    await SearchAndDownloadSubtitleAsync(movieFilePath);
                }
            }
            

            Console.WriteLine(string.Empty);
            Console.WriteLine("SubtitleDownloadCore Finished!");
        }

        private static List<string> GetMovieFiles(string rootDir)
        {
            Console.WriteLine("Movies Directory : " + rootDir);
            Console.WriteLine(string.Empty);

            List<string> files = new List<string>();

            string[] fileExtensions = { ".avi", ".mpg", ".mp4", ".mkv" };
                        
            foreach (string file in Directory.EnumerateFiles(rootDir, "*.*", SearchOption.AllDirectories)
                .Where(s => fileExtensions.Any(ext => ext == Path.GetExtension(s))))
            {
                files.Add(file);
            }

            return files;
        }

        private static async Task SearchAndDownloadSubtitleAsync(string filePath)
        {
            Console.WriteLine(string.Empty);

            string dir = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            if (File.Exists(Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath)) + ".srt"))
            {
                Console.WriteLine($"Subtitles already downloaded for {fileName}. Manually delete the .srt files to download again.");
                return;
            }

                        
            Console.WriteLine("Searching subtitle for " + filePath + " , wait...");
            
            using (HttpClient httpClient = new HttpClient())
            {
                string subdbApiFileHash = FileHashUtil.GetSubdbFileHash(filePath);

                string urlSearch = $"http://api.thesubdb.com/?action=search&hash={subdbApiFileHash}";

                httpClient.DefaultRequestHeaders.Add("User-Agent", "SubDB/1.0 (SubtitleDownloadCore/1.0; http://github.com/jaimemorais/SubtitleDownloadCore)");
                HttpResponseMessage searchResponse = await httpClient.GetAsync(urlSearch);

                if (searchResponse.IsSuccessStatusCode)
                {
                    string languagesFound = await searchResponse.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(languagesFound))
                    {
                        Console.WriteLine("Subtitle found (languages = " + languagesFound + ") !");

                        if (languagesFound.Contains(LANGUAGE_EN))
                        {
                            Console.WriteLine($"Downloading '{LANGUAGE_EN}' ... ");
                            await DownloadSubtitleAsync(subdbApiFileHash, dir, fileName, httpClient, LANGUAGE_EN);
                        }

                        if (languagesFound.Contains(LANGUAGE_PT))
                        {
                            Console.WriteLine($"Downloading '{LANGUAGE_PT}' ... ");
                            await DownloadSubtitleAsync(subdbApiFileHash, dir, fileName, httpClient, LANGUAGE_PT);
                        }

                        return;
                    }
                }
                
                Console.WriteLine("Subtitle not found :( ");                
            }
        }


        private static async Task DownloadSubtitleAsync(string subdbApiFileHash, string dir, string fileName, HttpClient httpClient, string language)
        {
            string urlDownload = $"http://api.thesubdb.com/?action=download&hash={subdbApiFileHash}&language="+language;
            HttpResponseMessage downloadResponse = await httpClient.GetAsync(urlDownload);

            if (downloadResponse.IsSuccessStatusCode)
            {
                HttpContent httpContent = downloadResponse.Content;
                var subtitleFilePath = dir + "\\" + fileName + ".srt";

                if (language.Equals(LANGUAGE_PT) && File.Exists(subtitleFilePath))
                {
                    subtitleFilePath = subtitleFilePath.Replace(".srt", "-pt.srt");
                }

                await WriteHttpContentToFile(httpContent, subtitleFilePath);

                Console.WriteLine("Subtitle downloaded -> " + subtitleFilePath);
            }
            else
            {
                Console.WriteLine("Failed to download. " + downloadResponse.StatusCode);
            }
        }
        

        private static Task WriteHttpContentToFile(HttpContent content, string srtFileName)
        {
            string subtitleFilePath = Path.GetFullPath(srtFileName);

            if (File.Exists(subtitleFilePath))
                File.Delete(subtitleFilePath);

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(subtitleFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                return content.CopyToAsync(fileStream).ContinueWith(
                    (copyTask) =>
                    {
                        fileStream.Close();
                    });
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
 
                throw;
            }
        }
        



        
    }
}
