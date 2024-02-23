using System.Diagnostics;

namespace LangChain.Providers.HuggingFace;

/// <summary>
/// 
/// </summary>
public class HuggingFaceChatModel(
    HuggingFaceProvider provider,
    string id)
    : ChatModel(id), IChatModel
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

    private static Message ToMessage(ICollection<GenerateTextResponseValue> message)
    {
        return new Message(
            Content: message.First().Generated_text,
            Role: MessageRole.Ai);
    }

    private async Task<ICollection<GenerateTextResponseValue>> CreateChatCompletionAsync(
        IReadOnlyCollection<Message> messages,
        CancellationToken cancellationToken = default)
    {
        return await provider.Api.GenerateTextAsync(modelId: Id, body: new GenerateTextRequest
        {
            Inputs = messages
                .Select(ToRequestMessage)
                .ToArray().AsPrompt(),
            Parameters = new GenerateTextRequestParameters
            {
                Max_new_tokens = 250,
            },
            Options = new GenerateTextRequestOptions
            {
                Use_cache = true,
                Wait_for_model = false,
            },
        }, cancellationToken).ConfigureAwait(false);
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
        var response = await CreateChatCompletionAsync(messages, cancellationToken).ConfigureAwait(false);

        messages.Add(ToMessage(response));

        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ChatResponse
        {
            Messages = messages,
            Usage = usage,
            UsedSettings = ChatSettings.Default,
        };
    }

    #endregion
}