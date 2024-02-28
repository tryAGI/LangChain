using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public abstract class AmazonTitanChatModel(
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
            var response = await provider.Api.InvokeModelWithResponseStreamAsync(streamRequest, cancellationToken);

            foreach (var payloadPart in response.Body)
            {
                var streamEvent = (PayloadPart)payloadPart;
                var chunk = await JsonSerializer.DeserializeAsync<JsonObject>(streamEvent.Bytes, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                var delta = chunk?["outputText"]!.GetValue<string>();

                OnPartialResponseGenerated(delta!);
                stringBuilder.Append(delta);

                var finished = chunk?["completionReason"]?.GetValue<string>();
                if (finished?.ToLower() == "finish")
                {
                    OnCompletedResponseGenerated(stringBuilder.ToString());
                }
            }

            OnPartialResponseGenerated(Environment.NewLine);
            stringBuilder.Append(Environment.NewLine);

            var newMessage = new Message(
                Content: stringBuilder.ToString(),
                Role: MessageRole.Ai);
            messages.Add(newMessage);

            OnCompletedResponseGenerated(newMessage.Content);
        }
        else
        {
            var response = await provider.Api.InvokeModelAsync(Id, bodyJson, cancellationToken)
                .ConfigureAwait(false);

            var generatedText = response?["results"]?[0]?["outputText"]?.GetValue<string>() ?? string.Empty;

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
            ["inputText"] = prompt,
            ["textGenerationConfig"] = new JsonObject
            {
                ["maxTokenCount"] = usedSettings.MaxTokens!.Value,
                ["temperature"] = usedSettings.Temperature!.Value,
                ["topP"] = usedSettings.TopP!.Value,
                ["stopSequences"] = usedSettings.StopSequences!.AsArray()
            }
        };
        return bodyJson;
    }
}