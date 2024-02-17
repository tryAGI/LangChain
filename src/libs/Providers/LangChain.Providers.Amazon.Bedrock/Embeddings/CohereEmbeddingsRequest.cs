using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Util;

namespace LangChain.Providers.Amazon.Bedrock.Embeddings;

public class CohereEmbeddingsRequest : IBedrockEmbeddingsRequest
{
    public async Task<float[][]> EmbedDocumentsAsync(
         AmazonBedrockRuntimeClient client,
         string[] texts,
         BedrockEmbeddingsConfiguration configuration)
    {
        texts = texts ?? throw new ArgumentNullException(nameof(texts));

        float[][] result = [];

        try
        {
            var payload = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(new
                {
                    texts = texts,
                    input_type = "search_document"
                })
            );

            InvokeModelResponse response = await client.InvokeModelAsync(new InvokeModelRequest()
            {
                ModelId = configuration.ModelId,
                Body = new MemoryStream(payload),
                ContentType = "application/json",
                Accept = "application/json"
            });

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var body = JsonNode.Parse(response.Body);
                var embeddings = body?["embeddings"]?
                    .AsArray()
                    .Select(x => x.AsArray()
                        .Select(y => (float)y.AsValue())
                        .ToArray())
                    .SelectMany(x => x);

                result = embeddings.Select(f => (new[] { f })).ToArray();
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

        return result;
    }

    public async Task<float[]> EmbedQueryAsync(
        AmazonBedrockRuntimeClient client,
        string text,
        BedrockEmbeddingsConfiguration configuration)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));
        float[]? embeddings = [];

        try
        {
            string payload = new JsonObject()
            {
                { "inputText", text },
            }.ToJsonString();

            InvokeModelResponse response = await client.InvokeModelAsync(new InvokeModelRequest()
            {
                ModelId = configuration.ModelId,
                Body = AWSSDKUtils.GenerateMemoryStreamFromString(payload),
                ContentType = "application/json",
                Accept = "application/json"
            });

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                embeddings = JsonNode.Parse(response.Body)?["embedding"]
                    .AsArray()
                    .Select(x => (float)x.AsValue())
                    .ToArray();
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

        return embeddings;
    }
}