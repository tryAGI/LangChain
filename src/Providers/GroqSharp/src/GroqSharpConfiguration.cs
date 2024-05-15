namespace LangChain.Providers.GroqSharp
{
    public class GroqSharpConfiguration
    {
        public const string SectionName = "GroqSharp";
        public string ApiKey { get; set; }
        public string ModelId { get; set; }
        public double Temperature { get; set; } = 0.5;
        public int MaxTokens { get; set; } = int.MaxValue;
        public double TopP { get; set; } = 1.0;
        public string Stop { get; set; } = "NONE";
        public int StructuredRetryPolicy { get; set; } = 5;
    }
}
