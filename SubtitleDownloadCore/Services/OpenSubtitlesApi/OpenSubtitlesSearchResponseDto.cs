using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SubtitleDownloadCore.Services.OpenSubtitlesApi
{

    public class OpenSubtitlesSearchResponseDto
    {
        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("data")]
        public List<Datum> Data { get; set; }
    }

    public class Attributes
    {
        [JsonPropertyName("subtitle_id")]
        public string SubtitleId { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("download_count")]
        public int DownloadCount { get; set; }

        [JsonPropertyName("new_download_count")]
        public int NewDownloadCount { get; set; }

        [JsonPropertyName("hearing_impaired")]
        public bool HearingImpaired { get; set; }

        [JsonPropertyName("hd")]
        public bool Hd { get; set; }

        [JsonPropertyName("fps")]
        public double Fps { get; set; }

        [JsonPropertyName("votes")]
        public int Votes { get; set; }

        [JsonPropertyName("ratings")]
        public double Ratings { get; set; }

        [JsonPropertyName("from_trusted")]
        public bool FromTrusted { get; set; }

        [JsonPropertyName("foreign_parts_only")]
        public bool ForeignPartsOnly { get; set; }

        [JsonPropertyName("upload_date")]
        public DateTime UploadDate { get; set; }

        [JsonPropertyName("ai_translated")]
        public bool AiTranslated { get; set; }

        [JsonPropertyName("machine_translated")]
        public bool MachineTranslated { get; set; }

        [JsonPropertyName("release")]
        public string Release { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("legacy_subtitle_id")]
        public int LegacySubtitleId { get; set; }

        [JsonPropertyName("uploader")]
        public Uploader Uploader { get; set; }

        [JsonPropertyName("feature_details")]
        public FeatureDetails FeatureDetails { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("related_links")]
        public RelatedLinks RelatedLinks { get; set; }

        [JsonPropertyName("files")]
        public List<File> Files { get; set; }

        [JsonPropertyName("moviehash_match")]
        public bool MoviehashMatch { get; set; }
    }

    public class Datum
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("attributes")]
        public Attributes Attributes { get; set; }
    }

    public class FeatureDetails
    {
        [JsonPropertyName("feature_id")]
        public int FeatureId { get; set; }

        [JsonPropertyName("feature_type")]
        public string FeatureType { get; set; }

        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("movie_name")]
        public string MovieName { get; set; }

        [JsonPropertyName("imdb_id")]
        public int ImdbId { get; set; }

        [JsonPropertyName("tmdb_id")]
        public int TmdbId { get; set; }
    }

    public class File
    {
        [JsonPropertyName("file_id")]
        public int FileId { get; set; }

        [JsonPropertyName("cd_number")]
        public int CdNumber { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }
    }

    public class RelatedLinks
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("img_url")]
        public string ImgUrl { get; set; }
    }


    public class Uploader
    {
        [JsonPropertyName("uploader_id")]
        public int? UploaderId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("rank")]
        public string Rank { get; set; }
    }




}
