//namespace LangChain.Providers.Anthropic;

///// <summary>
///// 
///// </summary>
//public partial class AnthropicModel : IChatModelWithTokenCounting
//{
//    #region Properties
    
//    private Tiktoken.Encoding Encoding { get; } = Tiktoken.Encoding.Get("cl100k_base");

//    #endregion

//    #region Methods

//    /// <inheritdoc/>
//    public int CountTokens(string text)
//    {
//        return Encoding.CountTokens(text);
//    }

//    /// <inheritdoc/>
//    public int CountTokens(IReadOnlyCollection<Message> messages)
//    {
//        return CountTokens(string.Join(
//            Environment.NewLine,
//            messages.Select(static x => x.Content)));
//    }

//    /// <inheritdoc/>
//    public int CountTokens(ChatRequest request)
//    {
//        request = request ?? throw new ArgumentNullException(nameof(request));
        
//        return CountTokens(request.Messages);
//    }

//    #endregion
//}