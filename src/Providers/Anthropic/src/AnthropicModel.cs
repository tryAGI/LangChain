using System.Diagnostics;
using LangChain.Providers.Anthropic.Extensions;

namespace LangChain.Providers.Anthropic;

/// <summary>
/// </summary>
public partial class AnthropicModel(
    AnthropicProvider provider,
    string id) : ChatModel(id), IPaidLargeLanguageModel
{
    #region Properties

    //public override int ContextLength => ApiHelpers.CalculateContextLength(Id);

    #endregion

    #region Methods

    private static global::Anthropic.Message ToRequestMessage(Message message)
    {
        return message.Role switch
        {
            MessageRole.System or MessageRole.Ai => new global::Anthropic.Message
            {
                Role = global::Anthropic.MessageRole.Assistant,
                Content = message.Content,
            },
            MessageRole.Human => new global::Anthropic.Message
            {
                Role = global::Anthropic.MessageRole.User,
                Content = message.Content,
            },
            MessageRole.FunctionCall => throw new NotImplementedException(),
            MessageRole.FunctionResult => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    private static Message ToMessage(global::Anthropic.Message message)
    {
        switch (message.Role)
        {
            case global::Anthropic.MessageRole.User:
                return new Message(string.Join("\r\n", message.Content.Value2!.Select(s => s.IsText ? s.Text!.Text : string.Empty)), MessageRole.Human);
            case global::Anthropic.MessageRole.Assistant:
                return new Message(string.Join("\r\n", message.Content.Value2!.Select(s => s.IsText ? s.Text!.Text : string.Empty)), MessageRole.Ai);
        }
        
        return new Message(string.Join("\r\n", message.Content.Value2!.Select(s => s.IsText ? s.Text!.Text : string.Empty)), MessageRole.Ai);

    }

    private Usage GetUsage(global::Anthropic.Message messages)
    {
        var completionTokens = messages.Usage?.OutputTokens ?? 0;

        var promptTokens = messages.Usage?.InputTokens ?? 0;

        var priceInUsd = CalculatePriceInUsd(
            outputTokens: completionTokens,
            inputTokens: promptTokens);

        return Usage.Empty with
        {
            InputTokens = promptTokens,
            OutputTokens = completionTokens,
            Messages = 1,
            PriceInUsd = priceInUsd
        };
    }

    /// <inheritdoc />
    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var messages = request.Messages.ToList();
        var watch = Stopwatch.StartNew();
        var systemMessage = messages.FirstOrDefault(m => m.Role == MessageRole.System).Content;
        var response = await provider.Api.CreateMessageAsync(
            model: Id,
            messages: messages
                .Select(ToRequestMessage)
                .ToList(),
            system: string.IsNullOrWhiteSpace(systemMessage) ? null : systemMessage,
            maxTokens: 4096,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var newMessage = ToMessage(response);
        messages.Add(newMessage);

        OnPartialResponseGenerated(newMessage.Content);
        OnPartialResponseGenerated(Environment.NewLine);
        OnCompletedResponseGenerated(newMessage.Content);

        var usage = GetUsage(response) with
        {
            Time = watch.Elapsed
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        // while (CallToolsAutomatically && newMessage.IsToolMessage())
        // {
        //     await CallFunctionsAsync(newMessage, messages, cancellationToken).ConfigureAwait(false);
        //
        //     if (ReplyToToolCallsAutomatically)
        //     {
        //         response = await provider.Api.CreateMessageAsync(
        //             model: Id,
        //             messages: messages
        //                 .Select(ToRequestMessage)
        //                 .ToList(),
        //             system: GetSystemMessage(),
        //             maxTokens: 4096,
        //             cancellationToken: cancellationToken).ConfigureAwait(false);
        //         newMessage = ToMessage(response);
        //         messages.Add(newMessage);
        //
        //         OnPartialResponseGenerated(newMessage.Content);
        //         OnPartialResponseGenerated(Environment.NewLine);
        //         OnCompletedResponseGenerated(newMessage.Content);
        //
        //         usage = GetUsage(response) with
        //         {
        //             Time = watch.Elapsed
        //         };
        //         AddUsage(usage);
        //         provider.AddUsage(usage);
        //     }
        // }

        return new ChatResponse
        {
            Messages = messages,
            Usage = usage,
            UsedSettings = ChatSettings.Default
        };
    }

    private static Task CallFunctionsAsync(Message newMessage, List<Message> messages,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
        // var function = newMessage.Content.ToAnthropicToolCall(GlobalTools);
        //
        // if (!string.IsNullOrEmpty(function.FunctionName))
        // {
        //     var call = Calls[function.FunctionName];
        //     var result = await call(function.Arguments?.ToString() ?? string.Empty, cancellationToken).ConfigureAwait(false);
        //     messages.Add(result.ToAnthropicToolResponseMessage(function.FunctionName));
        // }
    }

    /// <inheritdoc />
    public double CalculatePriceInUsd(int inputTokens, int outputTokens)
    {
        return CreateMessageRequestModelExtensions.ToEnum(Id)?.CalculatePriceInUsd(
            outputTokens,
            inputTokens) ?? 0;
    }

    #endregion
}