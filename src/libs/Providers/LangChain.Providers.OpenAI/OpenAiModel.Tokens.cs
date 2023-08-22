namespace LangChain.Providers;

// ReSharper disable MemberCanBePrivate.Global

public partial class OpenAiModel : ISupportsCountTokens
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public Tiktoken.Encoding Encoding { get; private set; }

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
        return CountTokens(request.Messages);
    }

    #endregion
}