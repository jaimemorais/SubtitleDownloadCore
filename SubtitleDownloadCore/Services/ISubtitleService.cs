using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubtitleDownloadCore.Services
{
    public interface ISubtitleService
    {
        /// <summary>
        /// Download the subtitles (languages PT and EN) for a movie file
        /// </summary>
        /// <param name="movieFilePath">The movie file path</param>
        /// <param name="srtFilePath">The subtitle (.srt) file path</param>
        /// <returns>A list containing the path of the downloaded subtitles</returns>
        Task<IList<string>> DownloadSubtitlesAsync(string movieFilePath, string srtFilePath);

    }
}
