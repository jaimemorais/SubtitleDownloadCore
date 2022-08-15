using System.Threading.Tasks;

namespace SubtitleDownloadCore.Services
{
    public interface ISubtitleService
    {

        Task DownloadSubtitlesAsync(string movieFilePath, string srtFilePath);

    }
}
