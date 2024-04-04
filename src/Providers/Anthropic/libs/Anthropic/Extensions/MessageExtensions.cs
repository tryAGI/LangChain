namespace LangChain.Providers.Anthropic.Extensions;

public static class MessageExtensions
{
    public static bool IsToolMessage(this Message message)
    {
        return message.Content.Contains("<function_calls>");
    }
}