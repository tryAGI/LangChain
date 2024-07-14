using Google.Cloud.AIPlatform.V1;
using Google.Protobuf.Collections;
using System.Diagnostics;

namespace LangChain.Providers.Google.VertexAI
{
    public class VertexAIChatModel(
        VertexAIProvider provider,
        string id
        ) : ChatModel(id), IChatModel
    {
        private VertexAIProvider Provider { get; } = provider ?? throw new ArgumentNullException(nameof(provider));
        public override async Task<ChatResponse> GenerateAsync(ChatRequest request,
            ChatSettings? settings = null,
            CancellationToken cancellationToken = default)
        {

            request = request ?? throw new ArgumentNullException(nameof(request));
            var prompt = ToPrompt(request.Messages);

            var watch = Stopwatch.StartNew();
            var response = await Provider.Api.GenerateContentAsync(prompt).ConfigureAwait(false);

            var result = request.Messages.ToList();
            result.Add(response.Candidates[0].Content.Parts[0].Text.AsAiMessage());

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
                UsedSettings = ChatSettings.Default,
            };

        }

        private GenerateContentRequest ToPrompt(IEnumerable<Message> messages)
        {
            var contents = new RepeatedField<Content>();
            foreach (var message in messages)
            {
                contents.Add(ConvertMessage(message));
            }

            return new GenerateContentRequest
            {
                Model = $"projects/{provider.Configuration.ProjectId}/locations/{provider.Configuration.Location}/publishers/{provider.Configuration.Publisher}/models/{Id}",
                Contents = { contents },
                GenerationConfig = provider.Configuration.GenerationConfig
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
