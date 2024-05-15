using GroqSharp.Models;
using System.Diagnostics;
using MessageGroqSharp = GroqSharp.Models.Message;

namespace LangChain.Providers.GroqSharp
{
    public class GroqSharpChatModel(
    GroqSharpProvider provider,
    string id)
    : ChatModel(id), IChatModel
    {
        public GroqSharpProvider Provider { get; } = provider ?? throw new ArgumentNullException(nameof(provider));
        public override async Task<ChatResponse> GenerateAsync(
            ChatRequest request,
            ChatSettings settings = null,
            CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var prompt = ToPrompt(request.Messages);
            var watch = Stopwatch.StartNew();
            var response = await Provider.Api.CreateChatCompletionAsync(prompt);

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

        protected static MessageGroqSharp[] ToPrompt(IEnumerable<Message> messages)
        {
            return messages.Select(ConvertMessage).ToArray();
        }

        protected static MessageGroqSharp ConvertMessage(Message message)
        {
            return new MessageGroqSharp { Role = ConvertRole(message.Role), Content = message.Content };
        }
        protected static MessageRoleType ConvertRole(MessageRole role)
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
}
