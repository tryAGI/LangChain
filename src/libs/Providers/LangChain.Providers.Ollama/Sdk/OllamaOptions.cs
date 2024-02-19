using System.Text.Json.Serialization;

namespace LangChain.Providers.Ollama;

/// <summary>
/// 
/// </summary>
public class OllamaOptions
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("num_keep")]
    public int? NumKeep { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("seed")]
    public int? Seed { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("num_predict")]
    public int? NumPredict { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("top_k")]
    public int? TopK { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("tfs_z")]
    public double? TfsZ { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("typical_p")]
    public double? TypicalP { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("repeat_last_n")]
    public int? RepeatLastN { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("repeat_penalty")]
    public double? RepeatPenalty { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("presence_penalty")]
    public double? PresencePenalty { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("frequency_penalty")]
    public double? FrequencyPenalty { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("mirostat")]
    public int? Mirostat { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("mirostat_tau")]
    public double? MirostatTau { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("mirostat_eta")]
    public double? MirostatEta { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("penalize_newline")]
    public bool? PenalizeNewline { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("stop")]
    public string[] Stop { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("numa")]
    public bool? Numa { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("num_ctx")]
    public int? NumCtx { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("num_batch")]
    public int? NumBatch { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("num_gqa")]
    public int? NumGqa { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("num_gpu")]
    public int? NumGpu { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("main_gpu")]
    public int? MainGpu { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("low_vram")]
    public bool? LowVram { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("f16_kv")]
    public bool? F16Kv { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("vocab_only")]
    public bool? VocabOnly { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("use_mmap")]
    public bool? UseMmap { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("use_mlock")]
    public bool? UseMlock { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("embedding_only")]
    public bool? EmbeddingOnly { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("rope_frequency_base")]
    public double? RopeFrequencyBase { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("rope_frequency_scale")]
    public double? RopeFrequencyScale { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("num_thread")]
    public int? NumThread { get; set; }
}