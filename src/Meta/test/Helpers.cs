using LangChain.Providers;
using LangChain.Providers.Anyscale;
using LangChain.Providers.Anyscale.Predefined;
using LangChain.Providers.Fireworks;
using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenAI.Predefined;
using LangChain.Providers.TogetherAi;

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
                    var llm = new Gpt4OmniMiniModel(provider);
                    var embeddings = new TextEmbeddingV3SmallModel(provider);

                    return (llm, embeddings);
                }
            case ProviderType.Together:
                {
                    // https://www.together.ai/blog/embeddings-endpoint-release
                    var provider = new TogetherAiProvider(
                        apiKey: Environment.GetEnvironmentVariable("TOGETHER_API_KEY") ??
                        throw new InconclusiveException("TOGETHER_API_KEY is not set"));
                    var llm = new TogetherAiModel(provider, id: "meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo");
                    var embeddings = new OpenAiEmbeddingModel(provider, id: "togethercomputer/m2-bert-80M-2k-retrieval");

                    return (llm, embeddings);
                }
            case ProviderType.Anyscale:
                {
                    // https://app.endpoints.anyscale.com/
                    var provider = new AnyscaleProvider(
                        apiKey: Environment.GetEnvironmentVariable("ANYSCALE_API_KEY") ??
                                throw new InconclusiveException("ANYSCALE_API_KEY is not set"));
                    var llm = new Llama2SmallModel(provider);

                    // Use OpenAI embeddings for now because Anyscale doesn't have embeddings yet
                    var embeddings = new TextEmbeddingV3SmallModel(
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                        throw new InconclusiveException("OPENAI_API_KEY is not set"));

                    return (llm, embeddings);
                }
            case ProviderType.Fireworks:
                {
                    // https://fireworks.ai/account/api-keys
                    var provider = new FireworksProvider(
                        apiKey: Environment.GetEnvironmentVariable("FIREWORKS_API_KEY") ??
                                throw new InconclusiveException("FIREWORKS_API_KEY is not set"));
                    var llm = new FireworksModel(provider, id: "accounts/fireworks/models/llama-v3p1-8b-instruct");

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