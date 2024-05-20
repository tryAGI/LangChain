using System.Diagnostics;
using GroqSharp.Models;
using MessageGroq = GroqSharp.Models.Message;

namespace LangChain.Providers.Groq;

public class GroqChatModel(
    GroqProvider provider,
    string id)
    : ChatModel(id), IChatModel
{
    private GroqProvider Provider { get; } = provider ?? throw new ArgumentNullException(nameof(provider));

    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        var prompt = ToPrompt(request.Messages);
        Provider.Api.SetModel(Id);
        var watch = Stopwatch.StartNew();
        var response = await Provider.Api.CreateChatCompletionAsync(prompt).ConfigureAwait(false);

        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        var result = request.Messages.ToList();
        result.Add(response.AsAiMessage());

        return new ChatResponse
        {
            Messages = result,
            Usage = usage,
            UsedSettings = ChatSettings.Default,
        };
    }

    private static MessageGroq[] ToPrompt(IEnumerable<Message> messages)
    {
        return messages.Select(ConvertMessage).ToArray();
    }

    private static MessageGroq ConvertMessage(Message message)
    {
        return new MessageGroq { Role = ConvertRole(message.Role), Content = message.Content };
    }

    private static MessageRoleType ConvertRole(MessageRole role)
    {
        return role switch
        {
            MessageRole.Human => MessageRoleType.User,
            MessageRole.Ai => MessageRoleType.Assistant,
            MessageRole.System => MessageRoleType.System,
            _ => throw new NotSupportedException($"the role {role} is not supported")
        };
    }
}