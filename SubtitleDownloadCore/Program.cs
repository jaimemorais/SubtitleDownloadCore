using Pastel;
using SubtitleDownloadCore.Services;
using SubtitleDownloadCore.Services.OpenSubtitlesApi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static System.Console;

namespace SubtitleDownloadCore
{

    public static class Program
    {
        private static readonly ISubtitleService _subtitleService = new OpenSubtitlesApiService();

        static readonly string[] MOVIEFILE_EXTENSIONS = { ".avi", ".mpg", ".mp4", ".mkv" };



        public static async Task Main(string[] args)
        {
            WriteLine($"{Environment.NewLine}SubtitleDownloadCore starting...{Environment.NewLine}".Pastel(Color.Yellow));

            string movieFilesDirectory = (args.Any() && !string.IsNullOrEmpty(args[0])) ? args[0] : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            await ExecuteDownloadAsync(movieFilesDirectory);

            WriteLine($"{Environment.NewLine}Finished!".Pastel(Color.Yellow));
        }



        private static async Task ExecuteDownloadAsync(string movieFilesDirectory)
        {
            WriteLine("Movie files directory : ".Pastel(Color.Yellow) + $"{movieFilesDirectory}{Environment.NewLine}".Pastel(Color.AntiqueWhite));

            var movieFiles = GetMovieFiles(movieFilesDirectory);

            if (movieFiles.Any())
            {
                await DownloadSubtitlesAsync(movieFiles);
            }
            else
            {
                WriteLine("No movie files found.".Pastel(Color.Tomato));
            }
        }

        private static async Task DownloadSubtitlesAsync(List<string> movieFiles)
        {
            foreach (string movieFilePath in movieFiles)
            {
                try
                {
                    WriteLine(string.Empty);

                    string srtFilePath = Path.Combine(Path.GetDirectoryName(movieFilePath), Path.GetFileNameWithoutExtension(movieFilePath)) + ".srt";
                    if (System.IO.File.Exists(srtFilePath))
                    {
                        WriteLine($"Subtitles already downloaded for {Path.GetFileNameWithoutExtension(movieFilePath)}. Manually delete the .srt files to download again.");
                        continue;
                    }

                    var downloadedSubtitles = await _subtitleService.DownloadSubtitlesAsync(movieFilePath, srtFilePath);

                    WriteResult(movieFilePath, downloadedSubtitles);
                }
                catch (Exception ex)
                {
                    WriteLine($"Unexpected error : {ex.Message}");
                }

            }
        }

        private static void WriteResult(string movieFilePath, IList<string> downloadedSubtitles)
        {
            if (downloadedSubtitles.Any())
            {
                WriteLine($"Subtitles downloaded for {movieFilePath} : ");
                downloadedSubtitles.ToList<string>().ForEach(s => WriteLine(s));
            }
            else
            {
                WriteLine($"Subtitles not found :( ");
            }
        }

        private static List<string> GetMovieFiles(string movieFilesDirectory) =>
            Directory.EnumerateFiles(movieFilesDirectory, "*.*", SearchOption.AllDirectories)
                .Where(s => MOVIEFILE_EXTENSIONS.Any(ext => ext == Path.GetExtension(s)))
                .ToList();


    }
}
