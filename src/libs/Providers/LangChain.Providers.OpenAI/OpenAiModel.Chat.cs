using System.Diagnostics;

namespace LangChain.Providers.OpenAI;

public partial class OpenAiModel : IChatModelWithTokenCounting
{
    #region Methods

    private static global::OpenAI.Chat.Message ToRequestMessage(Message message)
    {
        switch (message.Role)
        {
            case MessageRole.System:
            case MessageRole.Ai:
            case MessageRole.Human:
                return new global::OpenAI.Chat.Message(
                    role: message.Role switch
                    {
                        MessageRole.System => global::OpenAI.Chat.Role.System,
                        MessageRole.Ai => global::OpenAI.Chat.Role.Assistant,
                        MessageRole.Human => global::OpenAI.Chat.Role.User,
                        _ => global::OpenAI.Chat.Role.User,
                        
                    },
                    content: message.Content);
        }
        
        // Name = string.IsNullOrWhiteSpace(message.FunctionName)
        //     ? null
        //     : message.FunctionName,
        return new global::OpenAI.Chat.Message();
    }

    private static Message ToMessage(global::OpenAI.Chat.Message message)
    {
        return new Message(
            Content: message.Content, //message.Function_call?.Arguments ?? 
            Role: message.Role switch
            {
                global::OpenAI.Chat.Role.System => MessageRole.System,
                global::OpenAI.Chat.Role.User => MessageRole.Human,
                //global::OpenAI.Chat.Role.Assistant when message.Function_call != null => MessageRole.FunctionCall,
                global::OpenAI.Chat.Role.Assistant => MessageRole.Ai,
                //global::OpenAI.Chat.Role.Function => MessageRole.FunctionResult,
                _ => MessageRole.Human,
            }); //, FunctionName: message.Function_call?.Name
    }

    private async Task<global::OpenAI.Chat.ChatResponse> CreateChatCompletionAsync(
        IReadOnlyCollection<Message> messages,
        CancellationToken cancellationToken = default)
    {
        // Functions = GlobalFunctions.Count == 0
        //     ? null
        //     : GlobalFunctions,
        // Function_call = GlobalFunctions.Count == 0
        //     ? Function_call4.None
        //     : Function_call4.Auto,
        return await Api.ChatEndpoint.GetCompletionAsync(
            new global::OpenAI.Chat.ChatRequest(
                messages: messages
                    .Select(ToRequestMessage)
                    .ToArray(),
                model: Id,
                user: User),
            cancellationToken).ConfigureAwait(false);
    }

    private Usage GetUsage(global::OpenAI.Chat.ChatResponse response)
    {
        var completionTokens = response.Usage.CompletionTokens ?? 0;
        var promptTokens = response.Usage.PromptTokens ?? 0;
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

        // while (CallFunctionsAutomatically && message.Function_call != null)
        // {
        //     var functionName = message.Function_call.Name ?? string.Empty;
        //     var func = Calls[functionName];
        //     var json = await func(message.Function_call.Arguments ?? string.Empty, cancellationToken).ConfigureAwait(false);
        //     messages.Add(json.AsFunctionResultMessage(functionName));
        //
        //     if (ReplyToFunctionCallsAutomatically)
        //     {
        //         response = await CreateChatCompletionAsync(messages, cancellationToken).ConfigureAwait(false);
        //         message = response.GetFirstChoiceMessage();
        //         messages.Add(ToMessage(message));
        //         usage += GetUsage(response);
        //         lock (_usageLock)
        //         {
        //             TotalUsage += usage;
        //         }
        //     }
        // }

        return new ChatResponse(
            Messages: messages,
            Usage: usage);
    }

    #endregion
}