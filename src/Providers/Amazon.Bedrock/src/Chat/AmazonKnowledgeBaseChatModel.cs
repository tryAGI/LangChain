using System.Diagnostics;
using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public abstract class AmazonKnowledgeBaseChatModel(
    BedrockProvider provider,
    string id)
    : ChatModel(id)
{
    private readonly string _id = id;

    /// <summary>
    /// Generates a chat response based on the provided `ChatRequest`.
    /// </summary>
    /// <param name="request">The `ChatRequest` containing the input messages and other parameters.</param>
    /// <param name="settings">Optional `ChatSettings` to override the model's default settings.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A `ChatResponse` containing the generated messages and usage information.</returns>
    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var watch = Stopwatch.StartNew();
        var prompt = request.Messages.ToSimplePrompt();

        var usedSettings = AmazonKnowledgeBaseChatSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ChatSettings);

        var retrieveAndGenerateRequest = new RetrieveAndGenerateRequest
        {
            Input = new RetrieveAndGenerateInput { Text = prompt },
            RetrieveAndGenerateConfiguration = new RetrieveAndGenerateConfiguration
            {
                Type = RetrieveAndGenerateType.KNOWLEDGE_BASE,
                KnowledgeBaseConfiguration = new KnowledgeBaseRetrieveAndGenerateConfiguration
                {
                    KnowledgeBaseId = usedSettings?.KnowledgeBaseId,
                    ModelArn = _id,
                    RetrievalConfiguration = new KnowledgeBaseRetrievalConfiguration
                    {
                        VectorSearchConfiguration = new KnowledgeBaseVectorSearchConfiguration
                        {
                            OverrideSearchType = usedSettings?.SelectedSearchType,
                            Filter = usedSettings?.Filter
                        }
                    }
                }
            }
        };
        var response = await provider.AgentApi!.RetrieveAndGenerateAsync(retrieveAndGenerateRequest, cancellationToken)
            .ConfigureAwait(false);

        var result = request.Messages.ToList();
        result.Add(response.Output.Text.AsAiMessage());
        usedSettings!.Citations = response.Citations;

        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ChatResponse
        {
            Messages = result,
            UsedSettings = usedSettings,
            Usage = usage,
        };
    }
}