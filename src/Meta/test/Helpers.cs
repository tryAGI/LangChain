using LangChain.Providers;
using LangChain.Providers.Anthropic;
using LangChain.Providers.Anthropic.Predefined;
using LangChain.Providers.Anyscale;
using LangChain.Providers.Anyscale.Predefined;
using LangChain.Providers.Azure;
using LangChain.Providers.DeepInfra;
using LangChain.Providers.DeepSeek;
using LangChain.Providers.DeepSeek.Predefined;
using LangChain.Providers.Fireworks;
using LangChain.Providers.Google;
using LangChain.Providers.Google.Predefined;
using LangChain.Providers.Groq;
using LangChain.Providers.Groq.Predefined;
using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenAI.Predefined;
using LangChain.Providers.OpenRouter;
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
                    var llm = new OpenAiLatestFastChatModel(provider);
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
            case ProviderType.OpenRouter:
                {
                    var provider = new OpenRouterProvider(
                        apiKey: Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ??
                                throw new InconclusiveException("OPENROUTER_API_KEY is not set"));
                    var llm = new Providers.OpenRouter.Predefined.OpenAiGpt4OModel(provider);

                    // Use OpenAI embeddings for now because OpenRouter doesn't have embeddings yet
                    var embeddings = new TextEmbeddingV3SmallModel(
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                        throw new InconclusiveException("OPENAI_API_KEY is not set"));

                    return (llm, embeddings);
                }
            case ProviderType.DeepInfra:
                {
                    var provider = new DeepInfraProvider(
                        apiKey: Environment.GetEnvironmentVariable("DEEPINFRA_API_KEY") ??
                                throw new InconclusiveException("DEEPINFRA_API_KEY is not set"));
                    var llm = new Providers.DeepInfra.Predefined.MetaLlama318BInstructModel(provider);

                    // Use OpenAI embeddings for now because DeepInfra doesn't have embeddings yet
                    var embeddings = new TextEmbeddingV3SmallModel(
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                        throw new InconclusiveException("OPENAI_API_KEY is not set"));

                    return (llm, embeddings);
                }
            case ProviderType.Google:
                {
                    var provider = new GoogleProvider(
                        apiKey: Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ??
                                throw new InconclusiveException("GOOGLE_API_KEY is not set"),
                        httpClient: new HttpClient());
                    var llm = new GeminiProModel(provider);

                    // Use OpenAI embeddings for now because Google doesn't have embeddings yet
                    var embeddings = new TextEmbeddingV3SmallModel(
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                        throw new InconclusiveException("OPENAI_API_KEY is not set"));

                    return (llm, embeddings);
                }
            case ProviderType.Anthropic:
                {
                    var provider = new AnthropicProvider(
                        apiKey: Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ??
                                throw new InconclusiveException("ANTHROPIC_API_KEY is not set"));
                    var llm = new Claude35Sonnet(provider);

                    // Use OpenAI embeddings for now because Anthropic doesn't have embeddings yet
                    var embeddings = new TextEmbeddingV3SmallModel(
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                        throw new InconclusiveException("OPENAI_API_KEY is not set"));

                    return (llm, embeddings);
                }
            case ProviderType.Groq:
                {
                    var config = new GroqConfiguration()
                    {
                        ApiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY") ??
                                 throw new InconclusiveException("GROQ_API_KEY is not set.")
                    };

                    var provider = new GroqProvider(config);
                    var llm = new Llama370B(provider);

                    // Use OpenAI embeddings for now because Anthropic doesn't have embeddings yet
                    var embeddings = new TextEmbeddingV3SmallModel(
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                        throw new InconclusiveException("OPENAI_API_KEY is not set"));

                    return (llm, embeddings);
                }
            case ProviderType.DeepSeek:
                {
                    var apiKey =
                        Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY", EnvironmentVariableTarget.User) ??
                        throw new InvalidOperationException("DEEPSEEK_API_KEY is not set");
                    var llm = new DeepSeekCoderModel(new DeepSeekProvider(apiKey));

                    // Use OpenAI embeddings for now because Anthropic doesn't have embeddings yet
                    var embeddings = new TextEmbeddingV3SmallModel(
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                        throw new InconclusiveException("OPENAI_API_KEY is not set"));

                    return (llm, embeddings);
                }
            case ProviderType.Azure:
                {
                    var apiKey =
                        Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY", EnvironmentVariableTarget.User) ??
                        throw new InvalidOperationException("AZURE_OPENAI_API_KEY is not set");
                    var apiEndpoint =
                        Environment.GetEnvironmentVariable("AZURE_OPENAI_API_ENDPOINT", EnvironmentVariableTarget.User) ??
                        throw new InvalidOperationException("AZURE_OPENAI_API_ENDPOINT is not set");
                    var deploymentName =
                        Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME", EnvironmentVariableTarget.User) ??
                        throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME is not set");

                    var configuration = new AzureOpenAiConfiguration
                    {
                        Id = deploymentName,
                        ApiKey = apiKey,
                        Endpoint = apiEndpoint,
                    };
                    var provider = new AzureOpenAiProvider(configuration);
                    var llm = new AzureOpenAiChatModel(provider, deploymentName);

                    // Use OpenAI embeddings for now because Anthropic doesn't have embeddings yet
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