using System.Diagnostics;
using GenerativeAI.Types;
using LangChain.Providers.Extensions;

namespace LangChain.Providers.Models;

/// <summary>
/// 
/// </summary>
public class GenerativeModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string ApiKey { get; init; }

    /// <inheritdoc/>
    public Usage TotalUsage { get; private set; }

    private HttpClient HttpClient { get; set; }

    private GenerativeAiConfiguration? Configuration { get; set; }

    public GenerativeAI.Models.GenerativeModel Api { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GenerativeModel(GenerativeAiConfiguration configuration, HttpClient httpClient)
    {
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ApiKey = configuration.ApiKey ?? throw new ArgumentException("ApiKey is not defined", nameof(configuration));
        Id = configuration.ModelId ?? throw new ArgumentException("ModelId is not defined", nameof(configuration));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Configuration = configuration;
        InitClient();
    }

    /// <summary>
    /// Wrapper around OpenAI large language models.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="id"></param>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GenerativeModel(string apiKey, HttpClient httpClient, string id)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Id = id ?? throw new ArgumentNullException(nameof(id));
        InitClient();
    }

    #endregion

    #region Methods
    /// <summary>
    /// Initialize API Client
    /// </summary>
    protected void InitClient()
    {
        this.Api = new GenerativeAI.Models.GenerativeModel(this.ApiKey, Id, HttpClient);
    }

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
            Content: message.Text(),
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
        if (Configuration != null)
        {
            request.GenerationConfig = new GenerationConfig()
            {
                MaxOutputTokens = Configuration.MaxOutputTokens,
                TopK = Configuration.TopK,
                TopP = Configuration.TopP,
                Temperature = Configuration.Temperature
            };
        }
        return await Api.GenerateContentAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default)
    {
        var messages = request.Messages.ToList();
        var watch = Stopwatch.StartNew();
        var response = await CreateChatCompletionAsync(messages, cancellationToken).ConfigureAwait(false);

        messages.Add(ToMessage(response));

        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        TotalUsage += usage;

        return new ChatResponse(
            Messages: messages,
            Usage: usage);
    }

    #endregion
}