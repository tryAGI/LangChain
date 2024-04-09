using System.Diagnostics;
using System.Text.Json.Nodes;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class Ai21LabsJurassic2ChatModel(
    BedrockProvider provider,
    string id)
    : ChatModel(id)
{
    /// <summary>
    /// Generates a chat response based on the provided `ChatRequest`.
    /// </summary>
    /// <param name="request">The `ChatRequest` containing the input messages and other parameters.</param>
    /// <param name="settings">Optional `ChatSettings` to override the model's default settings.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A `ChatResponse` containing the generated messages and usage information.</returns>
    public override async Task<ChatResponse> GenerateAsync(
        ChatRequest request,
        ChatSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));

        var watch = Stopwatch.StartNew();
        var prompt = request.Messages.ToSimplePrompt();

        var usedSettings = Ai21LabsJurassic2ChatSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ChatSettings);

        var bodyJson = CreateBodyJson(prompt, usedSettings);

        var response = await provider.Api.InvokeModelAsync(Id, bodyJson, cancellationToken)
            .ConfigureAwait(false);

        var generatedText = response?["completions"]?
            .AsArray()[0]?["data"]?
            .AsObject()["text"]?.GetValue<string>() ?? "";

        var result = request.Messages.ToList();
        result.Add(generatedText.AsAiMessage());

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

    /// <summary>
    /// Creates the request body JSON for the Ai21Labs model based on the provided prompt and settings.
    /// </summary>
    /// <param name="prompt">The input prompt for the model.</param>
    /// <param name="usedSettings">The settings to use for the request.</param>
    /// <returns>A `JsonObject` representing the request body.</returns>
    private static JsonObject CreateBodyJson(string prompt, BedrockChatSettings usedSettings)
    {
        var bodyJson = new JsonObject
        {
            ["prompt"] = prompt,
            ["maxTokens"] = usedSettings.MaxTokens!.Value,
            ["temperature"] = usedSettings.Temperature!.Value,
            ["topP"] = usedSettings.TopP!.Value,
            ["stopSequences"] = usedSettings.StopSequences!.AsArray()
        };
        return bodyJson;
    }
}