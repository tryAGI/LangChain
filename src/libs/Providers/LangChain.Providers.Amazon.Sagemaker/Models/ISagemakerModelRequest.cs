namespace LangChain.Providers.Amazon.SageMaker.Models;

public interface ISagemakerModelRequest
{
    Task<string> GenerateAsync(HttpClient httpClient, ChatRequest request, SageMakerConfiguration configuration);
    string ToPrompt(IReadOnlyCollection<Message> messages);
}