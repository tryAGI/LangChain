using LangChain.Providers.OpenAI;

namespace LangChain.Providers.Fireworks;

public class FireworksProvider : OpenAiProvider
{
    public FireworksProvider(FireworksConfiguration configuration) : base(configuration)
    {
    }

    public FireworksProvider(string apiKey) : base(apiKey, customEndpoint: "https://api.fireworks.ai/inference/v1")
    {
    }
}