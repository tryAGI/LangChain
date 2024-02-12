using Amazon.BedrockRuntime;

namespace LangChain.Providers.Amazon.Bedrock.Models;

public abstract class BedrockModelBase : IChatModel
{
    private readonly Dictionary<string, Func<IBedrockModelRequest>> _requestTypes = new()
    {
        { AmazonModelIds.AI21LabsJurassic2MidV1, () => new Ai21LabsJurassic2ModelRequest() },
        { AmazonModelIds.AI21LabsJurassic2UltraV1, () => new Ai21LabsJurassic2ModelRequest() },

        { AmazonModelIds.AmazonTitanTextG1LiteV1, () => new AmazonTitanModelRequest() },
        { AmazonModelIds.AmazonTitanTextG1ExpressV1, () => new AmazonTitanModelRequest() }, 
        { AmazonModelIds.AmazonTitanImageGeneratorG1V1, () => new AmazonTitanImageRequest() },

        { AmazonModelIds.AnthropicClaude2_1, () => new AnthropicClaudeModelRequest() },
        { AmazonModelIds.AnthropicClaude2, () => new AnthropicClaudeModelRequest() },
        { AmazonModelIds.AnthropicClaude1_3, () => new AnthropicClaudeModelRequest() },
        { AmazonModelIds.AnthropicClaudeInstant1_2, () => new AnthropicClaudeModelRequest() },

        { AmazonModelIds.CohereCommand, () => new CohereCommandModelRequest() },
        { AmazonModelIds.CohereCommandLight, () => new CohereCommandModelRequest() },

        { AmazonModelIds.MetaLlama2Chat13B, () => new MetaLlama2ModelRequest() },
        { AmazonModelIds.MetaLlama2Chat70B, () => new MetaLlama2ModelRequest() },
        { AmazonModelIds.MetaLlama213B, () => new MetaLlama2ModelRequest() },
        { AmazonModelIds.MetaLlama270B, () => new MetaLlama2ModelRequest() },

        { AmazonModelIds.StabilityAISDXL0_8 , () => new StableDiffusionRequest() },
        { AmazonModelIds.StabilityAISDXL1_0 , () => new StableDiffusionRequest() },
    };
    private protected readonly object _usageLock = new();

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