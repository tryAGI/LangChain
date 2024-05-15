namespace LangChain.Providers.GroqSharp
{
    public class GroqSharpProvider : Provider
    {
        public IGroqClient Api { get; private set; }
        public GroqSharpProvider(GroqClient groqClient)
        : base(id: GroqSharpConfiguration.SectionName)
        {
            Api = groqClient ?? throw new ArgumentNullException(nameof(groqClient));
        }

        public GroqSharpProvider(GroqSharpConfiguration configuration)
        : base(id: GroqSharpConfiguration.SectionName)
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
}
