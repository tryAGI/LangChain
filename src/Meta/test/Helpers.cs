using LangChain.Providers;
using LangChain.Providers.Anthropic;
using LangChain.Providers.Anthropic.Predefined;
using LangChain.Providers.Anyscale;
using LangChain.Providers.Anyscale.Predefined;
using LangChain.Providers.Azure;
using LangChain.Providers.DeepSeek;
using LangChain.Providers.DeepSeek.Predefined;
using LangChain.Providers.Google;
using LangChain.Providers.Google.Predefined;
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
                    var llm = new OpenAiLatestFastChatModel(provider);
                    var embeddings = new TextEmbeddingV3SmallModel(provider);

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
                        ApiKey = apiKey,
                        Endpoint = apiEndpoint,
                        DeploymentID = deploymentName,
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