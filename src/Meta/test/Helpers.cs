using Microsoft.Extensions.AI;
using OpenAI;

namespace LangChain.IntegrationTests;

public static class Helpers
{
    public static (IChatClient ChatClient, IEmbeddingGenerator<string, Embedding<float>> EmbeddingGenerator) GetModels(ProviderType providerType)
    {
        switch (providerType)
        {
            case ProviderType.OpenAi:
                {
                    var apiKey =
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                        throw new InconclusiveException("OPENAI_API_KEY is not set");
                    var openAiClient = new OpenAIClient(apiKey);
                    IChatClient chatClient = openAiClient.GetChatClient("gpt-4o-mini").AsIChatClient();
                    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = openAiClient.GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

                    return (chatClient, embeddingGenerator);
                }
            case ProviderType.Anthropic:
                {
                    var anthropicApiKey =
                        Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ??
                        throw new InconclusiveException("ANTHROPIC_API_KEY is not set");
                    IChatClient chatClient = new Anthropic.AnthropicClient(anthropicApiKey);

                    // Use OpenAI embeddings since Anthropic doesn't have embeddings
                    var openAiApiKey =
                        Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                        throw new InconclusiveException("OPENAI_API_KEY is not set");
                    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OpenAIClient(openAiApiKey).GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

                    return (chatClient, embeddingGenerator);
                }

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
