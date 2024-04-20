using System.Text.Json.Serialization;

namespace LangChain.Providers.Suno.Sdk;

public class Clip
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("video_url")]
    public string VideoUrl { get; set; } = string.Empty;

    [JsonPropertyName("audio_url")]
    public string AudioUrl { get; set; } = string.Empty;

    [JsonPropertyName("image_url")]
    public object ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("image_large_url")]
    public object ImageLargeUrl { get; set; } = string.Empty;

    [JsonPropertyName("major_model_version")]
    public string MajorModelVersion { get; set; } = string.Empty;

    [JsonPropertyName("model_name")]
    public string ModelName { get; set; } = string.Empty;

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; } = new Metadata();

    [JsonPropertyName("is_liked")]
    public bool IsLiked { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("is_trashed")]
    public bool IsTrashed { get; set; }

    [JsonPropertyName("reaction")]
    public object Reaction { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("play_count")]
    public int PlayCount { get; set; }

    [JsonPropertyName("upvote_count")]
    public int UpvoteCount { get; set; }

    [JsonPropertyName("is_public")]
    public bool IsPublic { get; set; }
}