using Azure;
using Azure.AI.OpenAI;
using System.Diagnostics;

namespace LangChain.Providers.Azure;

/// <summary>
/// Wrapper around Azure OpenAI large language models
/// </summary>
public class AzureOpenAIModel : IChatModel
{
    /// <summary>
    /// Azure OpenAI API Key
    /// </summary>
    public string ApiKey { get; init; }

    /// <inheritdoc/>
    public Usage TotalUsage { get; private set; }

    /// <summary>
    /// Deployment name
    /// </summary>
    public string Id { get; init; }

    /// <inheritdoc/>
    public int ContextLength => Configurations.ContextSize;

    /// <summary>
    /// Azure OpenAI Resource URI
    /// </summary>
    public string Endpoint { get; set; }

    private AzureOpenAIConfiguration Configurations { get; }

    #region Constructors
    /// <summary>
    /// Wrapper around Azure OpenAI
    /// </summary>
    /// <param name="apiKey">API Key</param>
    /// <param name="endpoint">Azure Open AI Resource URI</param>
    /// <param name="id">Deployment Model name</param>
    /// <exception cref="ArgumentNullException"></exception>
    public AzureOpenAIModel(string apiKey, string endpoint, string id)
    {
        Configurations = new AzureOpenAIConfiguration();
        Id = id ?? throw new ArgumentNullException(nameof(id));
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }

    /// <summary>
    /// Wrapper around Azure OpenAI
    /// </summary>
    /// <param name="configuration">AzureOpenAIConfiguration</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public AzureOpenAIModel(AzureOpenAIConfiguration configuration)
    {
        Configurations = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ApiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is not defined", nameof(configuration));
        Id = configuration.Id ?? throw new ArgumentException("Deployment model Id is not defined", nameof(configuration));
        Endpoint = configuration.Endpoint ?? throw new ArgumentException("Endpoint is not defined", nameof(configuration));
    }
    #endregion

    #region Methods
    /// <inheritdoc/>
    public async Task<ChatResponse> GenerateAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var messages = request.Messages.ToList();
        var watch = Stopwatch.StartNew();
        var response = await CreateChatCompleteAsync(messages, cancellationToken).ConfigureAwait(false);

        messages.Add(ToMessage(response.Value));

        watch.Stop();

        var usage = GetUsage(response.Value.Usage) with
        {
            Time = watch.Elapsed,
        };
        TotalUsage += usage;

        return new ChatResponse(
            Messages: messages,
            Usage: usage);
    }

    private async Task<Response<ChatCompletions>> CreateChatCompleteAsync(IReadOnlyCollection<Message> messages, CancellationToken cancellationToken = default)
    {
        var chatCompletionOptions = new ChatCompletionsOptions(Id, messages.Select(ToRequestMessage))
        {
            MaxTokens = Configurations.MaxTokens,
            ChoiceCount = Configurations.ChoiceCount,
            Temperature = Configurations.Temperature,
        };

        var client = new OpenAIClient(new Uri(Endpoint), new AzureKeyCredential(ApiKey));
        return await client.GetChatCompletionsAsync(chatCompletionOptions, cancellationToken).ConfigureAwait(false);
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