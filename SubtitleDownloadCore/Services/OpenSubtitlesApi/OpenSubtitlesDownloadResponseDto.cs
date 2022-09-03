using System;
using System.Text.Json.Serialization;

namespace SubtitleDownloadCore.Services.OpenSubtitlesApi
{
    public class OpenSubtitlesDownloadResponseDto
    {
        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("requests")]
        public int Requests { get; set; }

        [JsonPropertyName("remaining")]
        public int Remaining { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("reset_time")]
        public string ResetTime { get; set; }

        [JsonPropertyName("reset_time_utc")]
        public DateTime ResetTimeUtc { get; set; }
    }


}
