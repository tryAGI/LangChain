using Amazon.BedrockRuntime;

namespace LangChain.Providers.Bedrock.Models;

public abstract class BedrockModelBase : IChatModel
{
    private readonly Dictionary<string, Func<IBedrockModelRequest>> _requestTypes;
    private protected readonly object _usageLock = new();

    protected BedrockModelBase()
    {
        _requestTypes = new Dictionary<string, Func<IBedrockModelRequest>>
        {
            { "ai21.j2-mid-v1", () => new Ai21LabsJurassic2ModelRequest() },
            { "ai21.j2-ultra-v1", () => new Ai21LabsJurassic2ModelRequest() },

            { "anthropic.claude-instant-v1", () => new AnthropicClaudeModelRequest() },
            { "anthropic.claude-v1", () => new AnthropicClaudeModelRequest() },
            { "anthropic.claude-v2", () => new AnthropicClaudeModelRequest() },
            { "anthropic.claude-v2:1", () => new AnthropicClaudeModelRequest() },

            // { "amazon.titan-text-express-v1", () => new AmazonTitanModelRequest() },         // TODO
            // { "amazon.titan-text-lite-v1", () => new AmazonTitanModelRequest() },            // TODO    
            // { "amazon.titan-image-generator-v1", () => new AmazonTitanModelRequest() },      // TODO    

            // { "cohere.command-text-v14", () => new CohereCommandModelRequest() },            // TODO
            // { "cohere.command-light-text-v14", () => new CohereCommandModelRequest() },      // TODO

            { "meta.llama2-13b-chat-v1", () => new MetaLlama2ModelRequest() },
            { "meta.llama2-70b-chat-v1", () => new MetaLlama2ModelRequest() },

            { "stability.stable-diffusion-xl-v0", () => new StableDiffusionRequest() },
            { "stability.stable-diffusion-xl-v1", () => new StableDiffusionRequest() },
        };
    }

    public abstract Task<ChatResponse> GenerateAsync(ChatRequest request, CancellationToken cancellationToken = default);

    public string? Id { get; init; }
    public Usage TotalUsage { get; set; }
    public int ContextLength { get; }

    protected async Task<string> CreateChatCompletionAsync(
        ChatRequest request,
        BedrockConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var bedrockModelRequest = _requestTypes[Id]();
        var client = new AmazonBedrockRuntimeClient(configuration.Region);
        var response = await bedrockModelRequest.GenerateAsync(client, request, configuration);

        return response;
    }
}