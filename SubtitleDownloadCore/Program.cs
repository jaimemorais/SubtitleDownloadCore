using Pastel;
using SubtitleDownloadCore.Services;
using SubtitleDownloadCore.Services.SubdbApi;
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
        private static readonly ISubtitleService _subtitleService = new SubdbApiService();

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
                await _subtitleService.DownloadSubtitlesAsync(movieFiles);
            }
            else
            {
                WriteLine("No movie files found.".Pastel(Color.Tomato));
            }
        }



        private static List<string> GetMovieFiles(string movieFilesDirectory) =>
            Directory.EnumerateFiles(movieFilesDirectory, "*.*", SearchOption.AllDirectories)
                .Where(s => MOVIEFILE_EXTENSIONS.Any(ext => ext == Path.GetExtension(s)))
                .ToList();


    }
}
