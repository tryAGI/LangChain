using System.Diagnostics;
using System.Text;
using System.Text.Json;
using OpenAI;
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

    protected virtual async Task CallFunctionsAsync(
        global::OpenAI.Chat.Message message,
        IList<Message> messages,
        CancellationToken cancellationToken = default)
    {
        message = message ?? throw new ArgumentNullException(nameof(message));
        messages = messages ?? throw new ArgumentNullException(nameof(messages));

        foreach (var tool in message.ToolCalls ?? [])
        {
            var func = Calls[tool.Function.Name];
            var json = await func(tool.Function.Arguments.ToString(), cancellationToken).ConfigureAwait(false);
            messages.Add(json.AsFunctionResultMessage(tool));
        }
    }

    [CLSCompliant(false)]
    protected virtual global::OpenAI.Chat.Message ToRequestMessage(Message message)
    {
        switch (message.Role)
        {
            case MessageRole.System:
            case MessageRole.Ai:
            case MessageRole.Human:
                return new global::OpenAI.Chat.Message(
                    role: message.Role switch
                    {
                        MessageRole.System => Role.System,
                        MessageRole.Ai => Role.Assistant,
                        MessageRole.Human => Role.User,
                        _ => Role.User,

                    },
                    content: message.Content);
            case MessageRole.FunctionCall:
                return new global::OpenAI.Chat.Message(
                    role: Role.Assistant, string.Empty, message.ToToolCalls());
            case MessageRole.FunctionResult:
                return new global::OpenAI.Chat.Message(message.GetTool(), message.Content);
        }

        return new global::OpenAI.Chat.Message();
    }

    protected virtual Message ToMessage(global::OpenAI.Chat.Message message)
    {
        message = message ?? throw new ArgumentNullException(nameof(message));

        var role = message.Role switch
        {
            Role.System => MessageRole.System,
            Role.User => MessageRole.Human,
            Role.Assistant when message.ToolCalls != null && message.ToolCalls.Count > 0 => MessageRole.FunctionCall,
            Role.Assistant => MessageRole.Ai,
            Role.Tool => MessageRole.FunctionResult,
            _ => MessageRole.Human,
        };

        var content = message.Content;
        // fix: message contains json element instead of string
        if (content is JsonElement { ValueKind: JsonValueKind.String } element)
        {
            content = element.GetString();
        }

        return new Message(
            Content: message.ToolCalls?.ElementAtOrDefault(0)?.Function.Arguments.ToJsonString() ?? content,
            Role: role,
            FunctionName: $"{message.ToolCalls?.ElementAtOrDefault(0)?.Function.Name}:{message.ToolCalls?.ElementAtOrDefault(0)?.Id}");
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
        var chatRequest = GlobalTools.Count == 0
            ? new global::OpenAI.Chat.ChatRequest(
                messages: request.Messages
                    .Select(ToRequestMessage)
                    .ToArray(),
                model: Id,
                seed: usedSettings.Seed,
                stops: usedSettings.StopSequences!.ToArray(),
                user: usedSettings.User ?? string.Empty,
                temperature: usedSettings.Temperature,
                frequencyPenalty: usedSettings.FrequencyPenalty,
                number: usedSettings.Number,
                maxTokens: usedSettings.MaxTokens,
                topP: usedSettings.TopP,
                presencePenalty: usedSettings.PresencePenalty,
                logitBias: usedSettings.LogitBias ?? new Dictionary<string, double>())
            : new global::OpenAI.Chat.ChatRequest(
                messages: request.Messages
                    .Select(ToRequestMessage)
                    .ToArray(),
                tools: GlobalTools,
                model: Id,
                stops: usedSettings.StopSequences!.ToArray(),
                user: usedSettings.User ?? string.Empty,
                temperature: usedSettings.Temperature,
                frequencyPenalty: usedSettings.FrequencyPenalty,
                number: usedSettings.Number,
                maxTokens: usedSettings.MaxTokens,
                topP: usedSettings.TopP,
                presencePenalty: usedSettings.PresencePenalty,
                logitBias: usedSettings.LogitBias ?? new Dictionary<string, double>());
        if (usedSettings.UseStreaming == true)
        {
            var enumerable = provider.Api.ChatEndpoint.StreamCompletionEnumerableAsync(
                chatRequest,
                cancellationToken).ConfigureAwait(false);

            var stringBuilder = new StringBuilder(capacity: 1024);
            await foreach (var response in enumerable)
            {
                var delta = response.Choices.ElementAt(0).Delta.Content;

                OnPartialResponseGenerated(delta);
                stringBuilder.Append(delta);
            }
            OnPartialResponseGenerated(Environment.NewLine);
            stringBuilder.Append(Environment.NewLine);

            var newMessage = new Message(
                Content: stringBuilder.ToString(),
                Role: MessageRole.Ai);
            messages.Add(newMessage);

            OnCompletedResponseGenerated(newMessage.Content);

            var usage = Usage.Empty with
            {
                Time = watch.Elapsed,
            };
            AddUsage(usage);
            provider.AddUsage(usage);

            return new ChatResponse
            {
                Messages = messages,
                UsedSettings = usedSettings,
                Usage = usage,
            };
        }
        else
        {
            var response = await provider.Api.ChatEndpoint.GetCompletionAsync(
                chatRequest,
                cancellationToken).ConfigureAwait(false);
            var message = response.GetFirstChoiceMessage();
            var newMessage = ToMessage(message);
            messages.Add(newMessage);

            OnPartialResponseGenerated(newMessage.Content);
            OnPartialResponseGenerated(Environment.NewLine);
            OnCompletedResponseGenerated(newMessage.Content);

            var usage = GetUsage(response) with
            {
                Time = watch.Elapsed,
            };
            AddUsage(usage);
            provider.AddUsage(usage);


            while (CallToolsAutomatically && message.ToolCalls != null && message.ToolCalls.Count > 0)
            {
                await CallFunctionsAsync(message, messages, cancellationToken).ConfigureAwait(false);

                if (ReplyToToolCallsAutomatically)
                {
                    chatRequest = new global::OpenAI.Chat.ChatRequest(
                        messages: messages
                            .Select(ToRequestMessage)
                            .ToArray(),
                        model: Id,
                        stops: usedSettings.StopSequences!.ToArray(),
                        user: usedSettings.User!,
                        temperature: usedSettings.Temperature,
                        tools: GlobalTools.Count == 0
                            ? null!
                            : GlobalTools);
                    response = await provider.Api.ChatEndpoint.GetCompletionAsync(
                        chatRequest,
                        cancellationToken).ConfigureAwait(false);
                    message = response.GetFirstChoiceMessage();
                    newMessage = ToMessage(message);
                    messages.Add(newMessage);

                    OnPartialResponseGenerated(newMessage.Content);
                    OnPartialResponseGenerated(Environment.NewLine);
                    OnCompletedResponseGenerated(newMessage.Content);

                    usage = GetUsage(response) with
                    {
                        Time = watch.Elapsed,
                    };
                    AddUsage(usage);
                    provider.AddUsage(usage);
                }
            }

            return new ChatResponse
            {
                Messages = messages,
                UsedSettings = usedSettings,
                Usage = usage,
            };
        }
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