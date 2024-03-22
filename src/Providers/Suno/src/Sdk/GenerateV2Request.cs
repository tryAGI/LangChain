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
}