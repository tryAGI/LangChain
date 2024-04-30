// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

// ReSharper disable MemberCanBePrivate.Global

public partial class OpenAiChatModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public Tiktoken.Encoding Encoding { get; } =
        Tiktoken.Encoding.TryForModel(chatModel.Id) ?? Tiktoken.Encoding.Get(Tiktoken.Encodings.Cl100KBase);

    #endregion

    #region Methods

    /// <inheritdoc/>
    public int CountTokens(string text)
    {
        return Encoding.CountTokens(text);
    }

    /// <inheritdoc/>
    public int CountTokens(IReadOnlyCollection<Message> messages)
    {
        return CountTokens(string.Join(
            Environment.NewLine,
            messages.Select(static x => x.Content)));
    }

    /// <inheritdoc/>
    public int CountTokens(ChatRequest request)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        return CountTokens(request.Messages);
    }

    #endregion
}