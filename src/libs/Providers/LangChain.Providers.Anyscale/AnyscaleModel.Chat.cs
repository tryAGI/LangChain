using System.Diagnostics;
using tryAGI.OpenAI;

namespace LangChain.Providers;

public partial class AnyscaleModel : IChatModelWithTokenCounting
{
    #region Methods

    private static ChatCompletionRequestMessage ToRequestMessage(Message message)
    {
        return new ChatCompletionRequestMessage
        {
            Role = message.Role switch
            {
                MessageRole.System => ChatCompletionRequestMessageRole.System,
                MessageRole.Ai => ChatCompletionRequestMessageRole.Assistant,
                MessageRole.FunctionCall => ChatCompletionRequestMessageRole.Assistant,
                MessageRole.Human => ChatCompletionRequestMessageRole.User,
                MessageRole.FunctionResult => ChatCompletionRequestMessageRole.Function,
                _ => ChatCompletionRequestMessageRole.User,
            },
            Name = string.IsNullOrWhiteSpace(message.FunctionName)
                ? null
                : message.FunctionName,
            Content = message.Content,
        };
    }

    private static Message ToMessage(ChatCompletionResponseMessage message)
    {
        return new Message(
            Content: message.Function_call?.Arguments ?? message.Content ?? string.Empty,
            Role: message.Role switch
            {
                ChatCompletionResponseMessageRole.System => MessageRole.System,
                ChatCompletionResponseMessageRole.User => MessageRole.Human,
                ChatCompletionResponseMessageRole.Assistant when message.Function_call != null => MessageRole.FunctionCall,
                ChatCompletionResponseMessageRole.Assistant => MessageRole.Ai,
                ChatCompletionResponseMessageRole.Function => MessageRole.FunctionResult,
                _ => MessageRole.Human,
            },
            FunctionName: message.Function_call?.Name);
    }

    private async Task<CreateChatCompletionResponse> CreateChatCompletionAsync(
        IReadOnlyCollection<Message> messages,
        CancellationToken cancellationToken = default)
    {
        return await Api.CreateChatCompletionAsync(new CreateChatCompletionRequest
        {
            Messages = messages
                .Select(ToRequestMessage)
                .ToArray(),
            Model = Id,
            User = User,
        }, cancellationToken).ConfigureAwait(false);
    }

    private Usage GetUsage(CreateChatCompletionResponse response)
    {
        var completionTokens = response.Usage?.Completion_tokens ?? 0;
        var promptTokens = response.Usage?.Prompt_tokens ?? 0;
        var priceInUsd = CalculatePriceInUsd(
            completionTokens: completionTokens,
            promptTokens: promptTokens);

        return Usage.Empty with
        {
            PromptTokens = promptTokens,
            CompletionTokens = completionTokens,
            Messages = 1,
            PriceInUsd = priceInUsd,
        };
    }

    /// <inheritdoc/>
    public async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default)
    {
        var messages = request.Messages.ToList();
        var watch = Stopwatch.StartNew();
        var response = await CreateChatCompletionAsync(messages, cancellationToken).ConfigureAwait(false);

        var message = response.GetFirstChoiceMessage();
        messages.Add(ToMessage(message));

        var usage = GetUsage(response) with
        {
            Time = watch.Elapsed,
        };
        lock (_usageLock)
        {
            TotalUsage += usage;
        }

        return new ChatResponse(
            Messages: messages,
            Usage: usage);
    }

    #endregion
}