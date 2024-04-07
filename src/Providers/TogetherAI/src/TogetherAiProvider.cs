using LangChain.Providers.OpenAI;

namespace LangChain.Providers.TogetherAi;

public class TogetherAiProvider : OpenAiProvider
{
    public TogetherAiProvider(TogetherAiConfiguration configuration) : base(configuration)
    {
    }

    public TogetherAiProvider(string apiKey) : base(apiKey, "api.together.xyz")
    {
    }
}