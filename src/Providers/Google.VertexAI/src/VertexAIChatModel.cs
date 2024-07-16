using Google.Cloud.AIPlatform.V1;
using System.Diagnostics;

namespace LangChain.Providers.Google.VertexAI
{
    public class VertexAIChatModel(
        VertexAIProvider provider,
        string id
        ) : ChatModel(id), IChatModel
    {
        private VertexAIProvider Provider { get; } = provider ?? throw new ArgumentNullException(nameof(provider));
        public override async Task<ChatResponse> GenerateAsync(ChatRequest request, ChatSettings? settings = null, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var prompt = ToPrompt(request.Messages);

            var watch = Stopwatch.StartNew();
            var response = await Provider.Api.GenerateContentAsync(prompt).ConfigureAwait(false);

            var result = request.Messages.Append(response.Candidates[0].Content.Parts[0].Text.AsAiMessage()).ToList();

            var usage = Usage.Empty with
            {
                Time = watch.Elapsed,
                InputTokens = response.UsageMetadata.PromptTokenCount,
                OutputTokens = response.UsageMetadata.CandidatesTokenCount
            };

            return new ChatResponse
            {
                Messages = result,
                Usage = usage,
                UsedSettings = settings ?? ChatSettings.Default
            };
        }

        private GenerateContentRequest ToPrompt(IEnumerable<Message> messages)
        {
            return new GenerateContentRequest
            {
                Model = $"projects/{Provider.Configuration.ProjectId}/locations/{Provider.Configuration.Location}/publishers/{Provider.Configuration.Publisher}/models/{Id}",
                Contents = { messages.Select(ConvertMessage) },
                GenerationConfig = Provider.Configuration.GenerationConfig
            };
        }

        private static Content ConvertMessage(Message message)
        {
            return new Content
            {
                Role = ConvertRole(message.Role),
                Parts = { new Part { Text = message.Content } }
            };
        }

        private static string ConvertRole(MessageRole role)
        {
            return role switch
            {
                MessageRole.Human => "USER",
                MessageRole.Ai => "MODEL",
                MessageRole.System => "SYSTEM",
                _ => throw new NotSupportedException($"the role {role} is not supported")
            };
        }
    }
}
