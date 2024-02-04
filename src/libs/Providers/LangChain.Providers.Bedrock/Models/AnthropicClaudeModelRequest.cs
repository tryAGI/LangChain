using System.Text;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Util;

namespace LangChain.Providers.Bedrock.Models;

public class AnthropicClaudeModelRequest : IBedrockModelRequest
{
    public async Task<string> GenerateAsync(AmazonBedrockRuntimeClient client, ChatRequest request, BedrockConfiguration configuration)
    {
        var prompt = ToPrompt(request.Messages);

        var payload = new JsonObject()
        {
            { "prompt", prompt },
            { "max_tokens_to_sample", configuration.MaxTokens },
            { "temperature", configuration.Temperature },
            { "stop_sequences", new JsonArray("\n\nHuman:") }
        }.ToJsonString();

        string generatedText = "";
        try
        {
            InvokeModelResponse response = await client.InvokeModelAsync(new InvokeModelRequest()
            {
                ModelId = configuration.ModelId,
                Body = AWSSDKUtils.GenerateMemoryStreamFromString(payload),
                ContentType = "application/json",
                Accept = "application/json"
            });

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                generatedText = JsonNode.Parse(response.Body)?["completion"]?
                    .GetValue<string>() ?? "";
            }
            else
            {
                Console.WriteLine("InvokeModelAsync failed with status code " + response.HttpStatusCode);
            }
        }
        catch (AmazonBedrockRuntimeException e)
        {
            Console.WriteLine(e.Message);
        }
        return generatedText;
    }

    public string ToPrompt(IReadOnlyCollection<Message> messages)
    {
        var sb = new StringBuilder();

        foreach (var item in messages)
        {
            switch (item.Role)
            {
                case MessageRole.Human:
                case MessageRole.System:
                case MessageRole.Chat:
                case MessageRole.FunctionCall:
                case MessageRole.FunctionResult:
                    sb.Append("Human: " + item.Content);
                    break;

                case MessageRole.Ai:
                    sb.Append("Assistant: " + item.Content);
                    break;

                default:
                    sb.Append("\n\nAssistant: ");
                    break;
            }
        }
        sb.Append("\n\nAssistant: ");
        return sb.ToString();
    }
}