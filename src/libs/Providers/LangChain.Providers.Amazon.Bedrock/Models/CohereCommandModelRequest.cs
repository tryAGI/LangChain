using System.Text;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Util;

namespace LangChain.Providers.Amazon.Bedrock.Models;

public class CohereCommandModelRequest : IBedrockModelRequest
{
    public async Task<string> GenerateAsync(AmazonBedrockRuntimeClient client, ChatRequest request, BedrockConfiguration configuration)
    {
        var prompt = ToPrompt(request.Messages);

        string payload = new JsonObject()
        {
            { "prompt", prompt },
            { "max_tokens", configuration.MaxTokens },
            { "temperature", configuration.Temperature },
            { "p",1  },
            { "k",0  },
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
                var body = JsonNode.Parse(response.Body);
                generatedText = body?["generations"]?[0]?["text"]?.GetValue<string>() ?? "";
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
            sb.Append(item.Content);
        }
        return sb.ToString();
    }
}