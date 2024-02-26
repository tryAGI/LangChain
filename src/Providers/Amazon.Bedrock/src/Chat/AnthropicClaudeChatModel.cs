using System.Diagnostics;
using System.Text.Json.Nodes;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class AnthropicClaudeChatModel(
    BedrockProvider provider,
    string id)
    : ChatModel(id)
{
    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var watch = Stopwatch.StartNew();
        var prompt = request.Messages.ToRolePrompt();

        var usedSettings = BedrockChatSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ChatSettings);
        var response = await provider.Api.InvokeModelAsync(
            Id,
            new JsonObject
            {
                ["prompt"] = prompt,
                ["max_tokens_to_sample"] = usedSettings.MaxTokens!.Value,
                ["temperature"] = usedSettings.Temperature!.Value,
                ["top_p"] = usedSettings.TopP!.Value,
                ["top_k"] = usedSettings.TopK!.Value,
                ["stop_sequences"] = new JsonArray("\n\nHuman:")
            },
            cancellationToken).ConfigureAwait(false);

        var generatedText = response?["completion"]?
            .GetValue<string>() ?? "";

        var result = request.Messages.ToList();
        result.Add(generatedText.AsAiMessage());

        // Unsupported
        var usage = Usage.Empty with
        {
            Time = watch.Elapsed,
        };
        AddUsage(usage);
        provider.AddUsage(usage);

        return new ChatResponse
        {
            Messages = result,
            UsedSettings = usedSettings,
            Usage = usage,
        };
    }
}