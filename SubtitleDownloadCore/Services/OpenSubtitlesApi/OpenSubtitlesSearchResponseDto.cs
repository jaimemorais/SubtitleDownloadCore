using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SubtitleDownloadCore.Services.OpenSubtitlesApi
{


    public record OpenSubtitlesSearchResponseDto(
        [property: JsonPropertyName("total_pages")] int TotalPages,
        [property: JsonPropertyName("total_count")] int TotalCount,
        [property: JsonPropertyName("page")] int Page,
        [property: JsonPropertyName("data")] IReadOnlyList<Datum> Data
    );


    public record Attributes(
        [property: JsonPropertyName("subtitle_id")] string SubtitleId,
        [property: JsonPropertyName("language")] string Language,
        [property: JsonPropertyName("download_count")] int DownloadCount,
        [property: JsonPropertyName("new_download_count")] int NewDownloadCount,
        [property: JsonPropertyName("hearing_impaired")] bool HearingImpaired,
        [property: JsonPropertyName("hd")] bool Hd,
        [property: JsonPropertyName("format")] string Format,
        [property: JsonPropertyName("fps")] double Fps,
        [property: JsonPropertyName("votes")] int Votes,
        [property: JsonPropertyName("points")] int Points,
        [property: JsonPropertyName("ratings")] double Ratings,
        [property: JsonPropertyName("from_trusted")] bool FromTrusted,
        [property: JsonPropertyName("foreign_parts_only")] bool ForeignPartsOnly,
        [property: JsonPropertyName("ai_translated")] bool AiTranslated,
        [property: JsonPropertyName("machine_translated")] bool MachineTranslated,
        [property: JsonPropertyName("upload_date")] DateTime UploadDate,
        [property: JsonPropertyName("release")] string Release,
        [property: JsonPropertyName("comments")] string Comments,
        [property: JsonPropertyName("legacy_subtitle_id")] int LegacySubtitleId,
        [property: JsonPropertyName("uploader")] Uploader Uploader,
        [property: JsonPropertyName("feature_details")] FeatureDetails FeatureDetails,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("related_links")] IReadOnlyList<RelatedLink> RelatedLinks,
        [property: JsonPropertyName("files")] IReadOnlyList<File> Files
    );

    public record Datum(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("attributes")] Attributes Attributes
    );

    public record FeatureDetails(
        [property: JsonPropertyName("feature_id")] int FeatureId,
        [property: JsonPropertyName("feature_type")] string FeatureType,
        [property: JsonPropertyName("year")] int Year,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("movie_name")] string MovieName,
        [property: JsonPropertyName("imdb_id")] int ImdbId,
        [property: JsonPropertyName("tmdb_id")] int TmdbId
    );

    public record File(
        [property: JsonPropertyName("file_id")] int FileId,
        [property: JsonPropertyName("cd_number")] int CdNumber,
        [property: JsonPropertyName("file_name")] string FileName
    );

    public record RelatedLink(
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("img_url")] string ImgUrl
    );

    public record Uploader(
        [property: JsonPropertyName("uploader_id")] int UploaderId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("rank")] string Rank
    );



}
