using Azure.AI.OpenAI;
using System.Diagnostics;

namespace LangChain.Providers.Azure;

/// <summary>
/// Wrapper around Azure OpenAI large language models
/// </summary>
public class AzureOpenAiChatModel(
    AzureOpenAiProvider provider,
    string id)
    : ChatModel(id), IChatModel
{
    /// <inheritdoc/>
    public override int ContextLength => provider.Configurations.ContextSize;

    #region Methods
    
    /// <inheritdoc/>
    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        
        var messages = request.Messages.ToList();
        var watch = Stopwatch.StartNew();
        var response = await provider.Client.GetChatCompletionsAsync(
            new ChatCompletionsOptions(Id, messages.Select(ToRequestMessage))
            {
                MaxTokens = provider.Configurations.MaxTokens,
                ChoiceCount = provider.Configurations.ChoiceCount,
                Temperature = provider.Configurations.Temperature,
            },
            cancellationToken).ConfigureAwait(false);
        
        messages.Add(ToMessage(response.Value));

        watch.Stop();

        var usage = GetUsage(response.Value.Usage) with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        //provider.AddUsage(usage);

        return new ChatResponse
        {
            Messages = messages,
            Usage = usage,
            UsedSettings = ChatSettings.Default,
        };
    }

    private static ChatRequestMessage ToRequestMessage(Message message)
    {
        return message.Role switch
        {
            MessageRole.System => new ChatRequestSystemMessage(message.Content),
            MessageRole.Ai => new ChatRequestAssistantMessage(message.Content),
            MessageRole.Human => new ChatRequestUserMessage(message.Content),
            MessageRole.FunctionCall => throw new NotImplementedException(),
            MessageRole.FunctionResult => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    private static Message ToMessage(ChatCompletions message)
    {
        return new Message(
            Content: message.Choices[0].Message.Content,
            Role: MessageRole.Ai);
    }

    private static Usage GetUsage(CompletionsUsage usage)
    {
        return Usage.Empty with
        {
            InputTokens = usage.PromptTokens,
            OutputTokens = usage.CompletionTokens
        };
    }
    
    #endregion
}