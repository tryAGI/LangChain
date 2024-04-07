using OpenAI.Constants;

namespace LangChain.Providers.DeepSeek;

public static class DeepSeekModels
{
    /// <summary>
    ///     Good at general tasks
    ///     Context Length 16k
    /// </summary>
    public const string DeepSeekChat = "deepseek-chat";

    /// <summary>
    ///     Good at coding tasks
    ///     Context Length 16k
    /// </summary>
    public const string DeepSeekCoder = "deepseek-coder";

    public static ChatModels GetModelById(string id)
    {
        switch (id)
        {
            case DeepSeekChat:
                return new ChatModels(DeepSeekChat, 16 * 1000, 0, 0);
            case DeepSeekCoder:
                return new ChatModels(DeepSeekCoder, 16 * 1000, 0, 0);
            default:
                throw new NotImplementedException("Not a valid DeepSeek model.");
        }
    }
}