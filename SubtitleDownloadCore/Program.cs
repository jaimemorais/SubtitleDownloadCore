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
            WriteLine("Searching and downloading subtitles, wait ... ".Pastel(Color.Yellow));

            var movieFiles = GetMovieFiles(movieFilesDirectory);

            if (!movieFiles.Any())
            {
                WriteLine("No movie files found.".Pastel(Color.Tomato));
                return;
            }

            await DownloadSubtitlesAsync(movieFiles);
        }

        private static async Task DownloadSubtitlesAsync(List<string> movieFiles)
        {
            foreach (string movieFilePath in movieFiles)
            {
                WriteLine(string.Empty);
                WriteLine("_________________________________________________________________________");
                WriteLine($"Movie file : ".Pastel(Color.Yellow) + movieFilePath);

                try
                {
                    string srtFilePath = Path.Combine(Path.GetDirectoryName(movieFilePath), Path.GetFileNameWithoutExtension(movieFilePath)) + ".srt";
                    if (System.IO.File.Exists(srtFilePath))
                    {
                        WriteLine($" -> Subtitles already downloaded for {Path.GetFileNameWithoutExtension(movieFilePath)}. Delete the .srt files to download again.".Pastel(Color.Yellow));
                        continue;
                    }

                    var downloadedSubtitles = await _subtitleService.DownloadSubtitlesAsync(movieFilePath, srtFilePath);

                    WriteResult(downloadedSubtitles);
                }
                catch (Exception ex)
                {
                    WriteLine($" -> Unexpected error : {ex.Message}".Pastel(Color.Tomato));
                }

            }
        }

        private static void WriteResult(IList<string> downloadedSubtitles)
        {
            if (!downloadedSubtitles.Any())
            {
                WriteLine($" -> No subtitles found   :( ".Pastel(Color.Yellow));
                return;
            }

            downloadedSubtitles.ToList<string>().ForEach(s => WriteLine($" -> {s}".Pastel(Color.Yellow)));
        }

        private static List<string> GetMovieFiles(string movieFilesDirectory) =>
            Directory.EnumerateFiles(movieFilesDirectory, "*.*", SearchOption.AllDirectories)
                .Where(s => MOVIEFILE_EXTENSIONS.Any(ext => ext == Path.GetExtension(s)))
                .ToList();


    }
}
