using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SubtitleDownloadCore
{

    /// <summary>
    /// Subtitle Downloader 
    /// Using http://thesubdb.com/api/
    /// </summary>
    public class Program
    {

        public static async Task Main(string[] args)
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("SubtitleDownloadCore starting...");
            Console.WriteLine(string.Empty);


            var movieFiles = GetMovieFiles();

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

        private static List<string> GetMovieFiles()
        {
            List<string> files = new List<string>();

            string[] extensions = { ".avi", ".mpg", ".mp4" };

            string currDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            foreach (string file in Directory.EnumerateFiles(currDir, "*.*", SearchOption.AllDirectories)
                .Where(s => extensions.Any(ext => ext == Path.GetExtension(s))))
            {
                files.Add(file);
            }

            return files;
        }

        private static async Task SearchAndDownloadSubtitleAsync(string filePath)
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine("Searching subtitle for " + filePath + " , wait...");
            
            string dir = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);            
            
            using (HttpClient httpClient = new HttpClient())
            {
                string subdbApiFileHash = GetSubdbFileHash(filePath);
                string urlSearch = $"http://api.thesubdb.com/?action=search&hash={subdbApiFileHash}";

                httpClient.DefaultRequestHeaders.Add("User-Agent", "SubDB/1.0 (SubtitleDownloadCore/1.0; http://github.com/jaimemorais/SubtitleDownloadCore)");
                HttpResponseMessage searchResponse = await httpClient.GetAsync(urlSearch);

                if (searchResponse.IsSuccessStatusCode)
                {
                    string languagesFound = await searchResponse.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(languagesFound))
                    {
                        Console.WriteLine("Subtitle found (languages = " + languagesFound + ") ! Downloading...");
                        await DownloadSubtitleAsync(subdbApiFileHash, dir, fileName, httpClient, languagesFound);                        
                        return;
                    }
                }
                
                Console.WriteLine("Subtitle not found :( ");                
            }
        }


        private static async Task DownloadSubtitleAsync(string subdbApiFileHash, string dir, string fileName, HttpClient httpClient, string languages)
        {
            string urlDownload = $"http://api.thesubdb.com/?action=download&hash={subdbApiFileHash}&language="+languages;
            HttpResponseMessage downloadResponse = await httpClient.GetAsync(urlDownload);

            if (downloadResponse.IsSuccessStatusCode)
            {
                HttpContent httpContent = downloadResponse.Content;
                var subTitlePath = dir + "\\" + fileName + ".srt";
                await WriteHttpContentToFile(httpContent, subTitlePath);

                Console.WriteLine("Subtitle downloaded -> " + subTitlePath);
            }
            else
            {
                Console.WriteLine("Failed to download. " + downloadResponse.StatusCode);
            }
        }
        

        private static Task WriteHttpContentToFile(HttpContent content, string filename)
        {
            string pathname = Path.GetFullPath(filename);
 
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
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
        

        public static byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        private static string GetSubdbFileHash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] fileArray = StreamToByteArray(stream);
                    
                    byte[] first64kb = fileArray.Take(64 * 1024).ToArray();
                    byte[] last64kb = fileArray.Reverse().Take(64 * 1024).Reverse().ToArray();
                    byte[] concatBytes = first64kb.Concat(last64kb).ToArray();

                    var hash = md5.ComputeHash(concatBytes);

                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
               }
            }
        }

    }
}
