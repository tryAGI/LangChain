using System.Diagnostics;
using Reka;

namespace LangChain.Providers.Reka;

public class RekaModel(
    RekaProvider provider,
    string id)
    : ChatModel(id)
{
    /// <inheritdoc/>
    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var watch = Stopwatch.StartNew();
        var messages = request.Messages.ToList();

        var usedSettings = ChatSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ChatSettings);

        var response = await provider.HttpClient.ChatAsync(new global::Reka.ChatRequest
        {
            Model_name = Id,
            Frequency_penalty = 1.0,
            Length_penalty = 1.0,
            Presence_penalty = 1.0,
            //Random_seed = new Random_seed(),
            Request_output_len = 2048,
            Runtime_top_k = 1024,
            Runtime_top_p = 0.95,
            Stop_words = [],
            Temperature = 0.9,
            Use_search_engine = false,
            Stream = false,
            Conversation_history = request.Messages.Select(message => new ChatRound
            {
                Type = message.Role switch
                {
                    MessageRole.Human => ChatRoundType.Human,
                    MessageRole.Ai => ChatRoundType.Model,
                    _ => throw new ArgumentOutOfRangeException(nameof(message), message.Role, "Invalid message role."),
                },
                Text = message.Content,
            }).ToList(),
        }, cancellationToken).ConfigureAwait(false);

        var generatedText = response.Text;
        messages.Add(generatedText.AsAiMessage());
        OnCompletedResponseGenerated(generatedText);

        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ChatResponse
        {
            Messages = messages,
            UsedSettings = usedSettings,
            Usage = usage,
        };
    }
}