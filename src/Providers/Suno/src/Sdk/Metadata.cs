using System.Text.Json.Serialization;

namespace LangChain.Providers.Suno.Sdk;

public class Metadata
{
    [JsonPropertyName("tags")]
    public string Tags { get; set; } = string.Empty;

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("gpt_description_prompt")]
    public string GptDescriptionPrompt { get; set; } = string.Empty;

    [JsonPropertyName("audio_prompt_id")]
    public object AudioPromptId { get; set; } = string.Empty;

    [JsonPropertyName("history")]
    public object History { get; set; } = string.Empty;

    [JsonPropertyName("concat_history")]
    public object ConcatHistory { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("duration")]
    public object Duration { get; set; } = string.Empty;

    [JsonPropertyName("refund_credits")]
    public object RefundCredits { get; set; } = string.Empty;

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    [JsonPropertyName("error_type")]
    public object ErrorType { get; set; } = string.Empty;

    [JsonPropertyName("error_message")]
    public object ErrorMessage { get; set; } = string.Empty;
}