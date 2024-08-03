using LangChain.Providers.OpenAI;
using OpenAI;

namespace LangChain.Providers.DeepInfra;

public class DeepInfraProvider : OpenAiProvider
{
    public DeepInfraProvider(DeepInfraConfiguration configuration) : base(configuration)
    {
    }

    public DeepInfraProvider(string apiKey) : base(apiKey, "api.deepinfra.com")
    {

    }
}