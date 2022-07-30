using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubtitleDownloadCore.Services
{
    public interface ISubtitleService
    {
        Task<string> SearchSubtitleAsync(string movieFilePath);

        Task DownloadSubtitlesAsync(List<string> movieFiles);

    }
}
