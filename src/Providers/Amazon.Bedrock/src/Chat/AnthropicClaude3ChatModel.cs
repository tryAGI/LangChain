using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using LangChain.Providers.Amazon.Bedrock.Internal;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class AnthropicClaude3ChatModel(
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
        var prompt = request.Messages.ToRolePrompt();
        var messages = request.Messages.ToList();

        var stringBuilder = new StringBuilder();

        var usedSettings = AnthropicChatSettings.Calculate(
            requestSettings: settings,
            modelSettings: Settings,
            providerSettings: provider.ChatSettings);

        var bodyJson = CreateBodyJson(prompt, usedSettings, request.Image);

        if (usedSettings.UseStreaming == true)
        {
            var streamRequest = BedrockModelRequest.CreateStreamRequest(Id, bodyJson);
            var response = await provider.Api.InvokeModelWithResponseStreamAsync(streamRequest, cancellationToken).ConfigureAwait(false);

            foreach (var payloadPart in response.Body)
            {
                var streamEvent = (PayloadPart)payloadPart;
                var chunk = await JsonSerializer.DeserializeAsync<JsonObject>(streamEvent.Bytes, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                var type = chunk?["type"]!.GetValue<string>().ToUpperInvariant();
                if (type == "CONTENT_BLOCK_DELTA")
                {
                    var delta = chunk?["delta"]?["text"]!.GetValue<string>();

                    OnPartialResponseGenerated(delta!);
                    stringBuilder.Append(delta);
                }
                if (type == "CONTENT_BLOCK_STOP")
                {
                    break;
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
            var response = await provider.Api.InvokeModelAsync(Id, bodyJson, cancellationToken).ConfigureAwait(false);

            var generatedText = response?["content"]?[0]?["text"]?.GetValue<string>() ?? "";

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

    /// <summary>
    /// Creates the request body JSON for the Anthropic model based on the provided prompt and settings.
    /// </summary>
    /// <param name="prompt">The input prompt for the model.</param>
    /// <param name="usedSettings">The settings to use for the request.</param>
    /// <param name="image">Binary image to use for the request.</param>
    /// <returns>A `JsonObject` representing the request body.</returns>
    private static JsonObject CreateBodyJson(
        string? prompt,
        AnthropicChatSettings usedSettings,
        BinaryData? image = null)
    {
        usedSettings = usedSettings ?? throw new ArgumentNullException(nameof(usedSettings));

        var bodyJson = new JsonObject
        {
            ["anthropic_version"] = "bedrock-2023-05-31",
            ["max_tokens"] = usedSettings.MaxTokens!.Value,
            ["messages"] = new JsonArray
            {
               new JsonObject
               {
                   ["role"] = "user",
                   ["content"] = new JsonArray
                   {
                       new JsonObject
                       {
                           ["type"] = "text",
                           ["text"] = prompt,
                       }
                   }
               }
            }
        };

        if (image != null)
        {
            var binaryData = BinaryData.FromBytes(image);
            var base64 = Convert.ToBase64String(binaryData.ToArray());
            var jsonImage = new JsonObject
            {
                ["type"] = "image",
                ["source"] = new JsonObject
                {
                    ["type"] = "base64",
                    ["media_type"] = image.MediaType,
                    ["data"] = base64
                }
            };

            var content = ((JsonArray)bodyJson["messages"]?[0]?["content"]!);
            content.Add(jsonImage);
        }

        return bodyJson;
    }
}