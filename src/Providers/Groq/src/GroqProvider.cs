namespace LangChain.Providers.Groq;

public class GroqProvider : Provider
{
    public IGroqClient Api { get; private set; }
    
    public GroqProvider(GroqClient groqClient)
        : base(id: GroqConfiguration.SectionName)
    {
        Api = groqClient ?? throw new ArgumentNullException(nameof(groqClient));
    }

    public GroqProvider(GroqConfiguration configuration)
        : base(id: GroqConfiguration.SectionName)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        var apiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is not defined", nameof(configuration));
        var apiModel = configuration.ModelId ?? throw new ArgumentException("ModelId is not defined", nameof(configuration));

        Api = new GroqClient(apiKey, apiModel)
            .SetTemperature(configuration.Temperature)
            .SetMaxTokens(configuration.MaxTokens)
            .SetTopP(configuration.TopP)
            .SetStop(configuration.Stop)
            .SetStructuredRetryPolicy(configuration.StructuredRetryPolicy);
    }
}