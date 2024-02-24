using System.Diagnostics;
using GenerativeAI.Types;
using LangChain.Providers.Google.Extensions;

namespace LangChain.Providers.Google;

/// <summary>
/// 
/// </summary>
public class GoogleChatModel(
    GoogleProvider provider,
    string id)
    : ChatModel(id)
{
    #region Properties

    /// <inheritdoc/>
    public override int ContextLength => 0;

    private GenerativeAI.Models.GenerativeModel Api { get; } = new(
        apiKey: provider.ApiKey,
        model: id,
        client: provider.HttpClient);

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
            MessageRole.FunctionCall => throw new NotImplementedException(),
            MessageRole.FunctionResult => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
    }

    private static Message ToMessage(EnhancedGenerateContentResponse message)
    {
        return new Message(
            Content: message.Text() ?? string.Empty,
            Role: MessageRole.Ai);
    }

    private async Task<EnhancedGenerateContentResponse> CreateChatCompletionAsync(
        IReadOnlyCollection<Message> messages,
        CancellationToken cancellationToken = default)
    {
        var request = new GenerateContentRequest()
        {
            Contents = messages.Select(ToRequestMessage).ToArray()
        };
        if (provider.Configuration != null)
        {
            request.GenerationConfig = new GenerationConfig()
            {
                MaxOutputTokens = provider.Configuration.MaxOutputTokens,
                TopK = provider.Configuration.TopK,
                TopP = provider.Configuration.TopP,
                Temperature = provider.Configuration.Temperature,
            };
        }
        return await Api.GenerateContentAsync(request, cancellationToken).ConfigureAwait(false);
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