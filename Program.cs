using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SubtitleDownloadCore
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            // Using http://thesubdb.com/api/


            string path = @"C:\teste\dexter.mp4";
            string hash = GetHash(path);



            
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "SubDB/1.0 (JaimeSubtitleDownloadCore/1.0; http://github.com/jaimemorais/SubtitleDownloadCore)");
                
                string urlSearch = $"http://api.thesubdb.com/?action=search&hash={hash}&language=en";

                HttpResponseMessage searchResponse = await httpClient.GetAsync(urlSearch);

                if (searchResponse.IsSuccessStatusCode)
                {
                    // subtitle found, so we download it

                    string urlDownload = $"http://api.thesubdb.com/?action=download&hash={hash}&language=en";
                    HttpResponseMessage downloadResponse = await httpClient.GetAsync(urlDownload);

                    HttpContent httpContent = downloadResponse.Content;
                    await WriteHttpContentToFile(httpContent, @"C:\teste\dexter.srt", true);

                }
            }
                

        }

        
        private static Task WriteHttpContentToFile(HttpContent content, string filename, bool overwrite)
        {
            string pathname = Path.GetFullPath(filename);
            if (!overwrite && File.Exists(filename))
            {
                throw new InvalidOperationException(string.Format("File {0} already exists.", pathname));
            }
 
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

        private static string GetHash(string filePath)
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
