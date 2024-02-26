﻿using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class Ai21LabsJurassic2ChatModel(
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
        var prompt = request.Messages.ToSimplePrompt();

        var usedSettings = BedrockChatSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ChatSettings);
        var response = await provider.Api.InvokeModelAsync(
            Id,
            new JsonObject
            {
                ["prompt"] = prompt,
                ["maxTokens"] = usedSettings.MaxTokens!.Value,
                ["temperature"] = usedSettings.Temperature!.Value,
                ["topP"] = usedSettings.TopP!.Value,
                ["stopSequences"] = usedSettings.StopSequences!.AsArray()
            },
            cancellationToken).ConfigureAwait(false);

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
}