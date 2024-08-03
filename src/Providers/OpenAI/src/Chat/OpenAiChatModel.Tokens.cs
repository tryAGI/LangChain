using Tiktoken;
using Tiktoken.Encodings;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

// ReSharper disable MemberCanBePrivate.Global

public partial class OpenAiChatModel
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public Encoder? Encoder { get; private set; }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public int CountTokens(string text)
    {
        Encoder ??= ModelToEncoder.TryFor(ChatModel) ?? new Encoder(new Cl100KBase());

        return Encoder.CountTokens(text);
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