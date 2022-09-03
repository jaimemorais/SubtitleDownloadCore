using System.Text.Json.Serialization;

namespace SubtitleDownloadCore.Services.OpenSubtitlesApi
{

    public class OpenSubtitlesDownloadRequestDto
    {
        [JsonPropertyName("file_id")]
        public int FileId { get; set; }

    }

}
