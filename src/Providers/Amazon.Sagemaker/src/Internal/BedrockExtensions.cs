using System.Text;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.SageMaker.Internal;

internal static class BedrockExtensions
{
    public static string ToSimplePrompt(this IReadOnlyCollection<Message> messages)
    {
        messages = messages ?? throw new ArgumentNullException(nameof(messages));
        
        var sb = new StringBuilder();

        foreach (var item in messages)
        {
            sb.Append(item.Content);
        }
        return sb.ToString();
    }
}