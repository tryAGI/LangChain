using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Util;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LangChain.Providers.Bedrock.Models;

public class StableDiffusionRequest : IBedrockModelRequest
{
    public async Task<string> GenerateAsync(AmazonBedrockRuntimeClient client, ChatRequest request, BedrockConfiguration configuration)
    {
        var prompt = ToPrompt(request.Messages);

        var payload = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(new
            {
                text_prompts = prompt.Select(x => new { text = prompt }).ToArray()
            })
        );

        string generatedText = "";
        try
        {
            InvokeModelResponse response = await client.InvokeModelAsync(new InvokeModelRequest()
            {
                ModelId = configuration.ModelId,
                Body = new MemoryStream(payload),
                ContentType = "application/json",
                Accept = "application/json"
            });

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var body = JsonNode.Parse(response.Body)?["artifacts"][0]["base64"];
                generatedText = $"data:image/jpeg;base64,{body}";
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