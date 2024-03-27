using System.Text.Json.Serialization;

namespace LangChain.Providers.Suno.Sdk;

public class GenerateV2Request
{
    [JsonPropertyName("gpt_description_prompt")]
    public string GptDescriptionPrompt { get; set; } = string.Empty;

    [JsonPropertyName("mv")]
    public string Mv { get; set; } = string.Empty;

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("make_instrumental")]
    public bool MakeInstrumental { get; set; }
    
    [JsonPropertyName("tags")]
    public string Tags { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("continue_clip_id")]
    public string ContinueClipId { get; set; } = string.Empty;

    [JsonPropertyName("continue_at")]
    public int ContinueAt { get; set; }
}