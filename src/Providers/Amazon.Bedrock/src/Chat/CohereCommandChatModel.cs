﻿using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public abstract class CohereCommandChatModel(
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
        var messages = request.Messages.ToList();

        var stringBuilder = new StringBuilder();

        var usedSettings = BedrockChatSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ChatSettings);

        var bodyJson = CreateBodyJson(prompt, usedSettings);

        if (usedSettings.UseStreaming == true)
        {
            var streamRequest = BedrockModelStreamRequest.Create(Id, bodyJson);
            var response = await provider.Api.InvokeModelWithResponseStreamAsync(streamRequest, cancellationToken).ConfigureAwait(false);

            foreach (var payloadPart in response.Body)
            {
                var streamEvent = (PayloadPart)payloadPart;
                var chunk = await JsonSerializer.DeserializeAsync<JsonObject>(streamEvent.Bytes, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                var delta = chunk?["generations"]?[0]?["text"]?.GetValue<string>() ?? string.Empty;

                OnPartialResponseGenerated(delta);
                stringBuilder.Append(delta);

                var finished = chunk?["generations"]?[0]?["finish_reason"]?.GetValue<string>() ?? string.Empty;
                if (string.Equals(finished.ToUpperInvariant(), "COMPLETE", StringComparison.Ordinal))
                {
                    OnCompletedResponseGenerated(stringBuilder.ToString());
                }
            }
        }
        else
        {
            var response = await provider.Api.InvokeModelAsync(Id, bodyJson, cancellationToken)
                .ConfigureAwait(false);

            var generatedText = response?["generations"]?[0]?["text"]?.GetValue<string>() ?? string.Empty;

            messages.Add(generatedText.AsAiMessage());
            OnCompletedResponseGenerated(generatedText);
        }

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

    private static JsonObject CreateBodyJson(string prompt, BedrockChatSettings usedSettings)
    {
        var bodyJson = new JsonObject
        {
            ["prompt"] = prompt,
            ["max_tokens"] = usedSettings.MaxTokens!.Value,
            ["temperature"] = usedSettings.Temperature!.Value,
            ["p"] = usedSettings.TopP!.Value,
            ["k"] = usedSettings.TopK!.Value,
        };
        return bodyJson;
    }
}