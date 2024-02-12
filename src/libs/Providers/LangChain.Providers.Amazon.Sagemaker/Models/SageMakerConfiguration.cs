namespace LangChain.Providers.Amazon.SageMaker.Models;

public class SageMakerConfiguration
{
    public string? ModelId { get; set; }
    public double Temperature { get; set; } = 0.7f;
    public int MaxTokens { get; set; } = 4096;
    public int MaxNewTokens { get; set; } = 256;
    public string? Url { get; set; }
}