using Amazon.BedrockRuntime;

namespace LangChain.Providers.Bedrock.Models;

public interface IBedrockModelRequest
{
    Task<string> GenerateAsync(AmazonBedrockRuntimeClient client, ChatRequest request, BedrockConfiguration configuration);
    string ToPrompt(IReadOnlyCollection<Message> messages);
}