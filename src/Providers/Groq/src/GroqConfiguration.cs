namespace LangChain.Providers.Groq;

public class GroqConfiguration
{
    public const string SectionName = "Groq";
    public string ApiKey { get; set; } = string.Empty;
    public string ModelId { get; set; } = string.Empty;
    public double Temperature { get; set; } = 0.5;
    public int MaxTokens { get; set; } = int.MaxValue;
    public double TopP { get; set; } = 1.0;
    public string Stop { get; set; } = "NONE";
    public int StructuredRetryPolicy { get; set; } = 5;
}