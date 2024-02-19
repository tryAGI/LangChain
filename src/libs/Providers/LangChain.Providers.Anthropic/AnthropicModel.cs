using System.Diagnostics;

namespace LangChain.Providers.Anthropic;

/// <summary>
/// 
/// </summary>
public partial class AnthropicModel(
    AnthropicProvider provider,
    string id) : ChatModel(id), IPaidLargeLanguageModel
{
    #region Properties
    
    /// <inheritdoc/>
    public override int ContextLength => ApiHelpers.CalculateContextLength(Id);

    #endregion

    #region Methods

    private static string ToRequestMessage(Message message)
    {
        return message.Role switch
        {
            MessageRole.System => message.Content.AsAssistantMessage(),
            MessageRole.Ai => message.Content.AsAssistantMessage(),
            MessageRole.Human => StringExtensions.AsHumanMessage(message.Content),
            MessageRole.FunctionCall => throw new NotImplementedException(),
            MessageRole.FunctionResult => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
    }

    private static Message ToMessage(CreateCompletionResponse message)
    {
        return new Message(
            Content: message.Completion,
            Role: MessageRole.Ai);
    }

    private Usage GetUsage(IReadOnlyCollection<Message> messages)
    {
        var completionTokens = CountTokens(messages.Last().Content);
        var promptTokens = CountTokens(messages
            .Take(messages.Count - 1)
            .Select(ToRequestMessage)
            .ToArray()
            .AsPrompt());
        var priceInUsd = CalculatePriceInUsd(
            outputTokens: completionTokens,
            inputTokens: promptTokens);

        return Usage.Empty with
        {
            InputTokens = promptTokens,
            OutputTokens = completionTokens,
            Messages = 1,
            PriceInUsd = priceInUsd,
        };
    }

    /// <inheritdoc/>
    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var messages = request.Messages.ToList();
        var watch = Stopwatch.StartNew();
        var response = await provider.Api.CompleteAsync(new CreateCompletionRequest
        {
            Prompt = messages
                .Select(ToRequestMessage)
                .ToArray().AsPrompt(),
            Max_tokens_to_sample = 100_000,
            Model = Id,
        }, cancellationToken).ConfigureAwait(false);

        messages.Add(ToMessage(response));

        var usage = GetUsage(messages) with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);

        return new ChatResponse
        {
            Messages = messages,
            Usage = usage,
            UsedSettings = ChatSettings.Default,
        };
    }

    /// <inheritdoc/>
    public double CalculatePriceInUsd(int inputTokens, int outputTokens)
    {
        return ApiHelpers.CalculatePriceInUsd(
            modelId: Id,
            completionTokens: outputTokens,
            promptTokens: inputTokens);
    }

    #endregion
}