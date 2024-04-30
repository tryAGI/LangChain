using LangChain.Providers;
using LangChain.Providers.Anyscale;
using LangChain.Providers.Anyscale.Predefined;
using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenAI.Predefined;

namespace LangChain.IntegrationTests;

public static class Helpers
{
    public static (IChatModel ChatModel, IEmbeddingModel EmbeddingModel) GetModels(ProviderType providerType)
    {
        switch (providerType)
        {
            case ProviderType.OpenAi:
                {
                    var provider = new OpenAiProvider(
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                        throw new InconclusiveException("OPENAI_API_KEY is not set"));
                    var llm = new Gpt35TurboModel(provider);
                    var embeddings = new TextEmbeddingV3SmallModel(provider);

                    return (llm, embeddings);
                }
            case ProviderType.Together:
                {
                    // https://www.together.ai/blog/embeddings-endpoint-release
                    var provider = new OpenAiProvider(
                        apiKey: Environment.GetEnvironmentVariable("TOGETHER_API_KEY") ??
                        throw new InconclusiveException("TOGETHER_API_KEY is not set"),
                        customEndpoint: "api.together.xyz");
                    var llm = new OpenAiChatModel(provider, id: "meta-llama/Llama-2-70b-chat-hf");
                    var embeddings = new OpenAiEmbeddingModel(provider, id: "togethercomputer/m2-bert-80M-2k-retrieval");

                    return (llm, embeddings);
                }
            case ProviderType.Anyscale:
                {
                    // https://app.endpoints.anyscale.com/
                    var provider = new AnyscaleProvider(
                        apiKey: Environment.GetEnvironmentVariable("ANYSCALE_API_KEY") ??
                                throw new InconclusiveException("ANYSCALE_API_KEY is not set"));
                    var llm = new Llama2LargeModel(provider);

                    // Use OpenAI embeddings for now because Anyscale doesn't have embeddings yet
                    var embeddings = new TextEmbeddingV3SmallModel(
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                        throw new InconclusiveException("OPENAI_API_KEY is not set"));

                    return (llm, embeddings);
                }

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}