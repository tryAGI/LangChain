using System.Diagnostics;
using System.Text;

#pragma warning disable CS3001 // Argument type is not CLS-compliant

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

public partial class OpenAiChatModel(
    OpenAiProvider provider,
    string id)
    : ChatModel(id),
        IChatModelWithTokenCounting,
        IPaidLargeLanguageModel,
        IChatModel<ChatRequest, ChatResponse, OpenAiChatSettings>
{
    public OpenAiChatModel(
        OpenAiProvider provider,
        CreateChatCompletionRequestModel id)
        : this(provider, id.ToValueString())
    {
    }

    #region Properties

    private string ChatModel { get; } = id;

    /// <inheritdoc/>
    public override int ContextLength => CreateChatCompletionRequestModelExtensions.ToEnum(ChatModel)?.GetContextLength() ?? 0;

    #endregion

    #region Methods

    protected virtual async Task CallFunctionsAsync(
        ChatCompletionResponseMessage message,
        IList<Message> messages,
        CancellationToken cancellationToken = default)
    {
        message = message ?? throw new ArgumentNullException(nameof(message));
        messages = messages ?? throw new ArgumentNullException(nameof(messages));

        foreach (var tool in message.ToolCalls ?? [])
        {
            var func = Calls[tool.Function.Name];
            var json = await func(tool.Function.Arguments, cancellationToken).ConfigureAwait(false);
            messages.Add(json.AsFunctionResultMessage(tool));
        }
    }

    [CLSCompliant(false)]
    protected virtual ChatCompletionRequestMessage ToRequestMessage(Message message)
    {
        switch (message.Role)
        {
            case MessageRole.System:
                return new ChatCompletionRequestSystemMessage
                {
                    Role = ChatCompletionRequestSystemMessageRole.System,
                    Content = message.Content,
                };
            case MessageRole.Ai:
                return new ChatCompletionRequestAssistantMessage
                {
                    Role = ChatCompletionRequestAssistantMessageRole.Assistant,
                    Content = message.Content,
                };
            case MessageRole.Human:
                return new ChatCompletionRequestUserMessage
                {
                    Role = ChatCompletionRequestUserMessageRole.User,
                    Content = message.Content,
                };
            case MessageRole.FunctionCall:
                return new ChatCompletionRequestAssistantMessage
                {
                    Role = ChatCompletionRequestAssistantMessageRole.Assistant,
                    Content = message.Content,
                    ToolCalls = message.ToToolCalls(),
                };
            case MessageRole.FunctionResult:
                var nameAndId = message.FunctionName?.Split(':') ??
                    throw new ArgumentException("Invalid functionCall name and id string");

                return new ChatCompletionRequestToolMessage
                {
                    Role = ChatCompletionRequestToolMessageRole.Tool,
                    Content = message.Content,
                    ToolCallId = nameAndId.ElementAtOrDefault(1) ?? string.Empty,
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(message));
        }
    }

    protected static IReadOnlyCollection<Message> ToMessages(ChatCompletionResponseMessage message)
    {
        message = message ?? throw new ArgumentNullException(nameof(message));

        var role = message.Role switch
        {
            ChatCompletionResponseMessageRole.Assistant => MessageRole.Ai,
            _ => MessageRole.Ai,
        };

        if (message.ToolCalls?.Count > 0)
        {
            var messages = new List<Message>();
            foreach (var call in message.ToolCalls)
            {
                messages.Add(new Message(
                    Content: call.Function.Arguments,
                    Role: MessageRole.FunctionCall,
                    FunctionName: $"{call.Function.Name}:{call.Id}"));
            }

            return messages;
        }

        return [new Message(
            Content: message.Content ?? string.Empty,
            Role: role)];
    }

    protected static T ToTool<T>(OpenApiSchema schema) where T : global::OpenAI.OpenApiSchema, new()
    {
        schema = schema ?? throw new ArgumentNullException(nameof(schema));

        return new T
        {
            Type = schema.Type,
            Description = schema.Description,
            Items = schema.Items != null
                ? ToTool<global::OpenAI.OpenApiSchema>(schema.Items)
                : null,
            Properties = schema.Properties
                .ToDictionary(
                    x => x.Key,
                    x => ToTool<global::OpenAI.OpenApiSchema>(x.Value)),
            Required = schema.Required,
            Enum = schema.Enum,
        };
    }

    private Usage GetUsage(CreateChatCompletionResponse response)
    {
        var outputTokens = response.Usage?.CompletionTokens ?? 0;
        var inputTokens = response.Usage?.PromptTokens ?? 0;
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
        var tools = request.Tools
            .Concat(GlobalTools)
            .Select(x => new ChatCompletionTool
            {
                Type = ChatCompletionToolType.Function,
                Function = new FunctionObject
                {
                    Name = x.Type,
                    Description = x.Description,
                    Parameters = x.Items != null
                        ? ToTool<FunctionParameters>(x.Items)
                        : new FunctionParameters(),
                },
            })
            .ToArray();
        tools = tools.Length > 0 ? tools : null;

        var chatRequest = new CreateChatCompletionRequest
        {
            Model = Id,
            Messages = request.Messages
                .Select(ToRequestMessage)
                .ToArray(),
            Seed = usedSettings.Seed,
            Stop = usedSettings.StopSequences!.ToArray(),
            User = usedSettings.User,
            Temperature = usedSettings.Temperature,
            FrequencyPenalty = usedSettings.FrequencyPenalty,
            N = usedSettings.Number,
            MaxTokens = usedSettings.MaxTokens,
            TopP = usedSettings.TopP,
            PresencePenalty = usedSettings.PresencePenalty,
            LogitBias = usedSettings.LogitBias,
            Tools = tools,
        };
        if (usedSettings.UseStreaming == true)
        {
            var enumerable = provider.Api.Chat.CreateChatCompletionAsStreamAsync(
                chatRequest,
                cancellationToken).ConfigureAwait(false);

            var stringBuilder = new StringBuilder(capacity: 1024);
            await foreach (var response in enumerable)
            {
                var delta = response.Choices.ElementAt(0).Delta;
                var deltaContent = delta.Content ?? string.Empty;

                OnPartialResponseGenerated(deltaContent);
                stringBuilder.Append(deltaContent);
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
            var response = await provider.Api.Chat.CreateChatCompletionAsync(
                chatRequest,
                cancellationToken).ConfigureAwait(false);
            var message = response.Choices.First().Message;
            var newMessages = ToMessages(message);
            messages.AddRange(newMessages);

            OnPartialResponseGenerated(newMessages.First().Content);
            OnPartialResponseGenerated(Environment.NewLine);
            OnCompletedResponseGenerated(newMessages.First().Content);

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
                    response = await provider.Api.Chat.CreateChatCompletionAsync(
                        messages: messages
                            .Select(ToRequestMessage)
                            .ToArray(),
                        model: Id,
                        stop: usedSettings.StopSequences!.ToArray(),
                        user: usedSettings.User!,
                        temperature: usedSettings.Temperature,
                        tools: tools,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                    message = response.Choices.First().Message;
                    newMessages = ToMessages(message);
                    messages.AddRange(newMessages);

                    OnPartialResponseGenerated(newMessages.First().Content);
                    OnPartialResponseGenerated(Environment.NewLine);
                    OnCompletedResponseGenerated(newMessages.First().Content);

                    var usage2 = GetUsage(response) with
                    {
                        Time = watch.Elapsed,
                    };
                    AddUsage(usage2);
                    provider.AddUsage(usage2);
                    usage += usage2;
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
        return CreateChatCompletionRequestModelExtensions
            .ToEnum(ChatModel)?
            .GetPriceInUsd(
                outputTokens: outputTokens,
                inputTokens: inputTokens) ?? double.NaN;
    }

    #endregion
}