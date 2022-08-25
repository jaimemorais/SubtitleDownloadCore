﻿using Pastel;
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
        private static readonly string[] MOVIEFILE_EXTENSIONS_TO_SEARCH = { "avi", "mpg", "mp4", "mkv" };

        private static readonly ISubtitleService _subtitleService = new OpenSubtitlesApiService();



        public static async Task Main(string[] args)
        {
            WriteLine(string.Empty);
            WriteLine($"SubtitleDownloadCore starting...{Environment.NewLine}".Pastel(Color.Yellow));

            string movieFilesDirectory = (args.Any() && !string.IsNullOrEmpty(args[0])) ? args[0] : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            await ExecuteDownloadAsync(movieFilesDirectory);

            WriteLine(string.Empty);
            WriteLine($"Finished!".Pastel(Color.Yellow));
        }


        private static async Task ExecuteDownloadAsync(string movieFilesDirectory)
        {
            WriteLine("Movie files directory : ".Pastel(Color.Yellow) + $"{movieFilesDirectory}".Pastel(Color.AntiqueWhite));

            var extensionsToSearch = string.Join(",", MOVIEFILE_EXTENSIONS_TO_SEARCH.Select(c => c));
            var movieFilesFound = Directory.EnumerateFiles(movieFilesDirectory, "*.*", SearchOption.AllDirectories).Where(x => extensionsToSearch.Any(x.EndsWith));

            if (movieFilesFound.Any())
            {
                WriteLine($"{Environment.NewLine}Searching and downloading subtitles, wait ... ".Pastel(Color.Yellow));
                await DownloadSubtitlesAsync(movieFilesFound);
            }
            else
            {
                WriteLine($" -> No movie files found ({extensionsToSearch}) in the directory.".Pastel(Color.OrangeRed));
            }
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

                    WriteDownloadResult(downloadedSubtitles);
                }
                catch (Exception ex)
                {
                    WriteLine($" -> Unexpected error : {ex.Message}".Pastel(Color.OrangeRed));
                }
            }
        }

        private static void WriteDownloadResult(IList<string> downloadedSubtitles)
        {
            if (!downloadedSubtitles.Any())
            {
                WriteLine($" -> No subtitles found   :( ".Pastel(Color.OrangeRed));
                return;
            }

            downloadedSubtitles.ToList<string>().ForEach(subtitleFile => WriteLine($" -> {subtitleFile}".Pastel(Color.Yellow)));
        }


    }
}
