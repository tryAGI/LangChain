namespace LangChain.Providers.TogetherAi.CodeGenerator.Main;

public class GenerationOptions
{
    public string OutputFolder { get; set; } = "Generated";
    public Func<string, string, string, string>? ReplaceEnumNameFunc { get; set; }
    public string TogetherAiApiKey { get; set; } = string.Empty;
}