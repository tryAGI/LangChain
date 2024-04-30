using System.Diagnostics;
using GenerativeAI.Models;
using GenerativeAI.Types;
using LangChain.Providers.Google.Extensions;

namespace LangChain.Providers.Google;

/// <summary>
/// </summary>
public partial class GoogleChatModel(
    GoogleProvider provider,
    string id)
    : ChatModel(id)
{
    #region Properties

    /// <inheritdoc />
    public override int ContextLength => 0;

    private GenerativeModel Api { get; } = new(
        provider.ApiKey,
        id,
        provider.HttpClient)
    {
        AutoCallFunction = false,
        AutoReplyFunction = false,
        AutoHandleBadFunctionCalls = false
    };

    #endregion

    #region Methods

    private static Content ToRequestMessage(Message message)
    {
        return message.Role switch
        {
            MessageRole.System => message.Content.AsModelContent(),
            MessageRole.Ai => message.Content.AsModelContent(),
            MessageRole.Human => message.Content.AsUserContent(),
            MessageRole.Chat => message.Content.AsUserContent(),
            MessageRole.FunctionCall => message.Content.AsFunctionCallContent(message.FunctionName ?? string.Empty),
            MessageRole.FunctionResult => message.Content.AsFunctionResultContent(message.FunctionName ?? string.Empty),
            _ => throw new NotImplementedException()
        };
    }

    private static Message ToMessage(EnhancedGenerateContentResponse message)
    {
        if (message.GetFunction() != null)
        {
            var function = message.GetFunction();

            return new Message(function?.Arguments.GetString() ?? string.Empty,
                MessageRole.FunctionCall, function?.Name);
        }

        return new Message(
            message.Text() ?? string.Empty,
            MessageRole.Ai);
    }

    private async Task<EnhancedGenerateContentResponse> CreateChatCompletionAsync(
        IReadOnlyCollection<Message> messages,
        CancellationToken cancellationToken = default)
    {
        var request = new GenerateContentRequest
        {
            Contents = messages.Select(ToRequestMessage).ToArray(),
            Tools = GlobalFunctions.ToGenerativeAiTools()
        };


        if (provider.Configuration != null)
            request.GenerationConfig = new GenerationConfig
            {
                MaxOutputTokens = provider.Configuration.MaxOutputTokens,
                TopK = provider.Configuration.TopK,
                TopP = provider.Configuration.TopP,
                Temperature = provider.Configuration.Temperature
            };
        return await Api.GenerateContentAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private async Task<Message> StreamCompletionAsync(IReadOnlyCollection<Message> messages,
        CancellationToken cancellationToken = default)
    {
        var request = new GenerateContentRequest
        {
            Contents = messages.Select(ToRequestMessage).ToArray()
        };
        if (provider.Configuration != null)
            request.GenerationConfig = new GenerationConfig
            {
                MaxOutputTokens = provider.Configuration.MaxOutputTokens,
                TopK = provider.Configuration.TopK,
                TopP = provider.Configuration.TopP,
                Temperature = provider.Configuration.Temperature
            };
        var res = await Api.StreamContentAsync(request, OnPartialResponseGenerated, cancellationToken)
            .ConfigureAwait(false);

        OnCompletedResponseGenerated(res);
        return new Message(
            res,
            MessageRole.Ai);
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
        OnPromptSent(request.Messages.AsHistory());
        var usedSettings = GoogleGeminiChatSettings.Calculate(
            settings,
            Settings,
            provider.ChatSettings);

        if (usedSettings.UseStreaming == true)
        {
            var message = await StreamCompletionAsync(messages, cancellationToken).ConfigureAwait(false);
            messages.Add(message);
        }
        else
        {
            var response = await CreateChatCompletionAsync(messages, cancellationToken).ConfigureAwait(false);

            var message = ToMessage(response);
            messages.Add(message);

            OnPartialResponseGenerated(response.Text() ?? string.Empty);
            OnPartialResponseGenerated(Environment.NewLine);
            OnCompletedResponseGenerated(response.Text() ?? string.Empty);

            // Unsupported
            var usage2 = Usage.Empty with
            {
                Time = watch.Elapsed
            };
            AddUsage(usage2);
            provider.AddUsage(usage2);

            //Handle Function Call
            while (ReplyToToolCallsAutomatically && response.IsFunctionCall())
            {
                var function = response.GetFunction();
                var name = function?.Name ?? string.Empty;

                if (Calls.TryGetValue(name, out var func))
                {
                    var args = function?.Arguments.GetString() ?? string.Empty;

                    var jsonResult = await func(args, cancellationToken).ConfigureAwait(false);
                    messages.Add(jsonResult.AsFunctionResultMessage(name));
                }
                else
                {
                    throw new ArgumentException("Invalid function name passed by Gemini");
                }

                if (ReplyToToolCallsAutomatically)
                {
                    response = await CreateChatCompletionAsync(messages, cancellationToken).ConfigureAwait(false);

                    message = ToMessage(response);

                    OnPartialResponseGenerated(message.Content);
                    OnPartialResponseGenerated(Environment.NewLine);
                    OnCompletedResponseGenerated(message.Content);

                    messages.Add(message);

                    usage2 = Usage.Empty with
                    {
                        Time = watch.Elapsed
                    };
                    AddUsage(usage2);
                    provider.AddUsage(usage2);
                }
            }
        }


        //Function Call


        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ChatResponse
        {
            Messages = messages,
            Usage = usage,
            UsedSettings = ChatSettings.Default
        };
    }

    private static Message ToFunctionCallMessage(string jsonResult, string functionName)
    {
        //var result = JsonSerializer.Deserialize<JsonNode>(jsonResult, SerializerOptions);
        //var content = new Content()
        //{
        //    Role = Roles.Function,
        //    Parts = new[]
        //    {
        //        new Part()
        //        {
        //            FunctionResponse = new ChatFunctionResponse()
        //            {
        //                Name = functionName,
        //                Response = new FunctionResponse() { Name = functionName, Content = jsonResult }
        //            }
        //        }
        //    }
        //};

        return new Message(jsonResult, MessageRole.FunctionResult, functionName);
    }

    #endregion
}