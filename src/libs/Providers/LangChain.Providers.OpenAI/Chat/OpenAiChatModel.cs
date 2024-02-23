using System.Diagnostics;
using System.Text.Json;
using OpenAI.Constants;

#pragma warning disable CS3001 // Argument type is not CLS-compliant

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

public partial class OpenAiChatModel(
    OpenAiProvider provider,
    ChatModels chatModel)
    : ChatModel(chatModel.Id),
        IChatModelWithTokenCounting,
        IPaidLargeLanguageModel,
        IChatModel<ChatRequest, ChatResponse, OpenAiChatSettings>
{
    #region Properties

    private ChatModels ChatModel { get; } = chatModel;
    
    /// <inheritdoc/>
    public override int ContextLength => ChatModel.ContextLength;

    #endregion

    #region Constructors

    public OpenAiChatModel(
        OpenAiProvider provider,
        string id)
        : this(provider, ChatModels.ById(id) ?? new ChatModels(
            Id: id,
            ContextLength: 0,
            PricePerOutputTokenInUsd: 0.0,
            PricePerInputTokenInUsd: 0.0))
    {
    }

    #endregion
    
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
                        MessageRole.System => global::OpenAI.Role.System,
                        MessageRole.Ai => global::OpenAI.Role.Assistant,
                        MessageRole.Human => global::OpenAI.Role.User,
                        _ => global::OpenAI.Role.User,
                        
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
        var role = message.Role switch
        {
            global::OpenAI.Role.System => MessageRole.System,
            global::OpenAI.Role.User => MessageRole.Human,
            //global::OpenAI.Chat.Role.Assistant when message.Function_call != null => MessageRole.FunctionCall,
            global::OpenAI.Role.Assistant => MessageRole.Ai,
            //global::OpenAI.Chat.Role.Function => MessageRole.FunctionResult,
            _ => MessageRole.Human,
        };
        
        var content= message.Content;
        // fix: message contains json element instead of string
        if (content is JsonElement { ValueKind: JsonValueKind.String } element)
        {
            content = element.GetString();
        }
        
        return new Message(
            Content: content, //message.Function_call?.Arguments ?? 
            Role: role); //, FunctionName: message.Function_call?.Name
    }

    private Usage GetUsage(global::OpenAI.Chat.ChatResponse response)
    {
        var outputTokens = response.Usage.CompletionTokens ?? 0;
        var inputTokens = response.Usage.PromptTokens ?? 0;
        var priceInUsd = CalculatePriceInUsd(
            outputTokens: outputTokens,
            inputTokens: inputTokens);

        return Usage.Empty with
        {
            InputTokens = inputTokens,
            OutputTokens = outputTokens,
            Messages = 1,
            PriceInUsd = priceInUsd,
        };
    }

    /// <inheritdoc cref="IChatModel.GenerateAsync(ChatRequest, ChatSettings, CancellationToken)"/>
    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var messages = request.Messages.ToList();
        var watch = Stopwatch.StartNew();
        
        OnPromptSent(request.Messages.AsHistory());

        var usedSettings = OpenAiChatSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ChatSettings);
        // Functions = GlobalFunctions.Count == 0
        //     ? null
        //     : GlobalFunctions,
        // Function_call = GlobalFunctions.Count == 0
        //     ? Function_call4.None
        //     : Function_call4.Auto,
        var response = await provider.Api.ChatEndpoint.GetCompletionAsync(
            new global::OpenAI.Chat.ChatRequest(
                messages: request.Messages
                    .Select(ToRequestMessage)
                    .ToArray(),
                model: Id,
                stops: usedSettings.StopSequences!.ToArray(),
                user: usedSettings.User!,
                temperature: usedSettings.Temperature),
            cancellationToken).ConfigureAwait(false);
        
        var message = response.GetFirstChoiceMessage();
        var newMessage = ToMessage(message);
        messages.Add(newMessage);

        OnPartialResponseGenerated(newMessage.Content);
        OnCompletedResponseGenerated(newMessage.Content);
        
        var usage = GetUsage(response) with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

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

        return new ChatResponse
        {
            Messages = messages,
            UsedSettings = usedSettings,
            Usage = usage,
        };
    }

    public Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        OpenAiChatSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync(request, (ChatSettings?)settings, cancellationToken);
    }

    /// <inheritdoc/>
    public double CalculatePriceInUsd(int inputTokens, int outputTokens)
    {
        return ChatModel
            .GetPriceInUsd(
                outputTokens: outputTokens,
                inputTokens: inputTokens);
    }
    
    #endregion
}