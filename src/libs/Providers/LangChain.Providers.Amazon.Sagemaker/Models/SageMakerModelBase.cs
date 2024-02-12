namespace LangChain.Providers.Amazon.SageMaker.Models;

public abstract class SageMakerModelBase : IChatModel
{
    private protected readonly object _usageLock = new();

    public abstract Task<ChatResponse> GenerateAsync(ChatRequest request, CancellationToken cancellationToken = default);

    public string? Id { get; init; }
    public string? Url { get; init; }

    public Usage TotalUsage { get; set; }
    public int ContextLength { get; }
    public HttpClient HttpClient { get; set; } = new HttpClient();

    protected async Task<string> CreateChatCompletionAsync(
        ChatRequest request,
        SageMakerConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var sagemakerModelRequest = new SageMakerModelRequest();
        var response = await sagemakerModelRequest.GenerateAsync(HttpClient, request, configuration);

        return response;
    }
}