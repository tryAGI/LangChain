using Amazon.BedrockRuntime;

namespace LangChain.Providers.Bedrock.Embeddings;

public interface IBedrockEmbeddingsRequest
{
    Task<float[][]> EmbedDocumentsAsync(AmazonBedrockRuntimeClient client, string[] texts, BedrockEmbeddingsConfiguration configuration);
    Task<float[]> EmbedQueryAsync(AmazonBedrockRuntimeClient client, string text, BedrockEmbeddingsConfiguration configuration);
}