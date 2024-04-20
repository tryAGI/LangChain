using System.Diagnostics;
using Anthropic.SDK.Messaging;
using LangChain.Providers.Anthropic.Extensions;

namespace LangChain.Providers.Anthropic;

/// <summary>
/// </summary>
public partial class AnthropicModel(
    AnthropicProvider provider,
    string id) : ChatModel(id), IPaidLargeLanguageModel
{
    #region Properties

    /// <inheritdoc />
    //public override int ContextLength => ApiHelpers.CalculateContextLength(Id);

    #endregion

    #region Methods

    private static global::Anthropic.SDK.Messaging.Message ToRequestMessage(Message message)
    {
        return message.Role switch
        {
            MessageRole.System => message.Content.AsAssistantMessage(),
            MessageRole.Ai => message.Content.AsAssistantMessage(),
            MessageRole.Human => StringExtensions.AsHumanMessage(message.Content),
            MessageRole.FunctionCall => throw new NotImplementedException(),
            MessageRole.FunctionResult => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    private static Message ToMessage(MessageResponse message)
    {
        switch (message.Role)
        {
            case "user":
                return new Message(string.Join("\r\n", message.Content.Select(s => s.Text)), MessageRole.Human);
            case "assistant":
                return new Message(string.Join("\r\n", message.Content.Select(s => s.Text)), MessageRole.Ai);
        }

        return new Message(
            string.Join("\r\n", message.Content.Select(s => s.Text)),
            MessageRole.Ai);
    }

    private Usage GetUsage(MessageResponse messages)
    {
        var completionTokens = messages.Usage.OutputTokens;

        var promptTokens = messages.Usage.InputTokens;

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
        var response = await provider.Api.Messages.GetClaudeMessageAsync(new MessageParameters
        {
            Messages = messages
                .Select(ToRequestMessage)
                .ToList(),
            SystemMessage = GetSystemMessage(),
            MaxTokens = 4096,
            Model = Id
        }, cancellationToken).ConfigureAwait(false);

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


        while (CallToolsAutomatically && newMessage.IsToolMessage())
        {
            await CallFunctionsAsync(newMessage, messages);

            if (ReplyToToolCallsAutomatically)
            {
                response = await provider.Api.Messages.GetClaudeMessageAsync(new MessageParameters
                {
                    Messages = messages
                        .Select(ToRequestMessage)
                        .ToList(),
                    SystemMessage = GetSystemMessage(),
                    MaxTokens = 4096,
                    Model = Id
                }, cancellationToken).ConfigureAwait(false);
                newMessage = ToMessage(response);
                messages.Add(newMessage);

                OnPartialResponseGenerated(newMessage.Content);
                OnPartialResponseGenerated(Environment.NewLine);
                OnCompletedResponseGenerated(newMessage.Content);

                usage = GetUsage(response) with
                {
                    Time = watch.Elapsed
                };
                AddUsage(usage);
                provider.AddUsage(usage);
            }
        }

        return new ChatResponse
        {
            Messages = messages,
            Usage = usage,
            UsedSettings = ChatSettings.Default
        };
    }

    private async Task CallFunctionsAsync(Message newMessage, List<Message> messages,
        CancellationToken cancellationToken = default)
    {
        var function = newMessage.Content.ToAnthropicToolCall(GlobalTools);

        if (!string.IsNullOrEmpty(function.FunctionName))
        {
            var call = Calls[function.FunctionName];
            var result = await call(function.Arguments.ToString(), cancellationToken);
            messages.Add(result.ToAnthropicToolResponseMessage(function.FunctionName));
        }
    }

    /// <inheritdoc />
    public double CalculatePriceInUsd(int inputTokens, int outputTokens)
    {
        return ApiHelpers.CalculatePriceInUsd(
            Id,
            outputTokens,
            inputTokens);
    }

    #endregion
}