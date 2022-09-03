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
        public const string USER_AGENT = "subtitledownloadcore";
        public const string LANGUAGE_EN = "en";
        public const string LANGUAGE_PT = "pt";

        private static readonly string[] MOVIEFILE_EXTENSIONS_TO_SEARCH = { "*.avi", "*.mpg", "*.mp4", "*.mkv" };

        private static readonly ISubtitleService _subtitleService = new OpenSubtitlesApiService();



        public static async Task Main(string[] args)
        {
            WriteLine(string.Empty);
            WriteLine($"SubtitleDownloadCore starting...{Environment.NewLine}".Pastel(Color.Yellow));

            string movieFilesDirectory = (args.Any() && !string.IsNullOrEmpty(args[0])) ? args[0] : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            WriteLine("Movie files directory : ".Pastel(Color.Yellow) + $"{movieFilesDirectory}".Pastel(Color.AntiqueWhite));


            var movieFilesFound = MOVIEFILE_EXTENSIONS_TO_SEARCH
                .AsParallel()
                .SelectMany(extension => Directory.EnumerateFiles(movieFilesDirectory, extension, SearchOption.AllDirectories));

            WriteLine(movieFilesFound.Any() ?
                $"{Environment.NewLine}Searching and downloading subtitles, wait ... ".Pastel(Color.Yellow) :
                $" -> No movie files found in the directory.".Pastel(Color.OrangeRed));

            if (movieFilesFound.Any())
            {
                await DownloadSubtitlesAsync(movieFilesFound);
            }

            WriteLine(string.Empty);
            WriteLine($"Finished!".Pastel(Color.Yellow));
        }


        private static async Task DownloadSubtitlesAsync(IEnumerable<string> movieFiles)
        {
            foreach (string movieFilePath in movieFiles)
            {
                WriteLine(string.Empty);
                WriteLine("_________________________________________________________________________");
                WriteLine($"Movie file : ".Pastel(Color.Yellow) + movieFilePath.Pastel(Color.AntiqueWhite));

                try
                {
                    string srtFilePath = Path.Combine(Path.GetDirectoryName(movieFilePath), Path.GetFileNameWithoutExtension(movieFilePath)) + ".srt";
                    if (System.IO.File.Exists(srtFilePath))
                    {
                        WriteLine($" -> Subtitles already downloaded for {Path.GetFileNameWithoutExtension(movieFilePath)}. Delete the .srt files to download again.".Pastel(Color.Yellow));
                        continue;
                    }

                    var downloadedSubtitles = await _subtitleService.DownloadSubtitlesAsync(movieFilePath, srtFilePath);

                    if (downloadedSubtitles.Any())
                    {
                        downloadedSubtitles.ToList().ForEach(subtitleFile => WriteLine($" -> {subtitleFile}".Pastel(Color.Yellow)));
                    }
                    else
                    {
                        WriteLine($" -> No subtitles found   :( ".Pastel(Color.OrangeRed));
                    }
                }
                catch (Exception ex)
                {
                    WriteLine($" -> Unexpected error : {ex.Message}".Pastel(Color.OrangeRed));
                }
            }
        }


    }
}
