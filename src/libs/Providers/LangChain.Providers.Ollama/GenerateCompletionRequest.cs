using System.Text.Json.Serialization;

namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public class GenerateCompletionRequest
{
    /// <summary>
    /// The model name (required)
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// The prompt to generate a response for
    /// </summary>
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Additional model parameters listed in the documentation for the Modelfile such as temperature
    /// </summary>
    [JsonPropertyName("options")]
    public OllamaLanguageModelOptions Options { get; set; } = new();

    /// <summary>
    /// Base64-encoded images (for multimodal models such as llava)
    /// </summary>
    [JsonPropertyName("images")]
    public string[] Images { get; set; }

    /// <summary>
    /// System prompt to (overrides what is defined in the Modelfile)
    /// </summary>
    [JsonPropertyName("system")]
    public string System { get; set; } = string.Empty;

    /// <summary>
    /// The full prompt or prompt template (overrides what is defined in the Modelfile)
    /// </summary>
    [JsonPropertyName("template")]
    public string Template { get; set; } = string.Empty;

    /// <summary>
    /// The context parameter returned from a previous request to /generate, this can be used to keep a short conversational memory
    /// </summary>
    [JsonPropertyName("context")]
    public long[] Context { get; set; } 

    /// <summary>
    /// If false the response will be returned as a single response object, rather than a stream of objects
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = true;

    /// <summary>
    /// In some cases you may wish to bypass the templating system and provide a full prompt. In this case, you can use the raw parameter to disable formatting.
    /// </summary>
    [JsonPropertyName("raw")]
    public bool Raw { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; }
}

/// <summary>
/// 
/// </summary>
public class GenerateCompletionResponseStream
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("done")]
    public bool Done { get; set; }
}

/// <summary>
/// 
/// </summary>
public class GenerateCompletionDoneResponseStream : GenerateCompletionResponseStream
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("context")]
    public long[] Context { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("total_duration")]
    public long TotalDuration { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("load_duration")]
    public long LoadDuration { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("prompt_eval_count")]
    public int PromptEvalCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("prompt_eval_duration")]
    public long PromptEvalDuration { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("eval_count")]
    public int EvalCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("eval_duration")]
    public long EvalDuration { get; set; }
}