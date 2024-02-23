using LangChain.Providers.OpenAI;

namespace LangChain.Providers.Anyscale;

public class AnyscaleProvider : OpenAiProvider
{
    public AnyscaleProvider(AnyscaleConfiguration configuration) : base(configuration)
    {
    }
    
    public AnyscaleProvider(string apiKey) : base(apiKey, customEndpoint: "https://api.endpoints.anyscale.com/v1")
    {
    }
}