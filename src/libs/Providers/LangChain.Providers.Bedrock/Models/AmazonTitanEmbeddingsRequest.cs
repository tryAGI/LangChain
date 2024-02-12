using System.Text.Json.Nodes;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Util;
using LangChain.Providers.Bedrock.Embeddings;
using LangChain.TextSplitters;

namespace LangChain.Providers.Bedrock.Models;

public class AmazonTitanEmbeddingsRequest : IBedrockEmbeddingsRequest
{
    public async Task<float[][]> EmbedDocumentsAsync(
        AmazonBedrockRuntimeClient client,
        string[] texts,
        BedrockEmbeddingsConfiguration configuration)
    {
        texts = texts ?? throw new ArgumentNullException(nameof(texts));

        List<float> arrEmbeddings = [];
        var inputText = string.Join(" ", texts);
        var textSplitter = new RecursiveCharacterTextSplitter(chunkSize: 10_000);
        var splitText = textSplitter.SplitText(inputText);

        foreach (var text in splitText)
        {
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
                    var embeddings = JsonNode.Parse(response.Body)?["embedding"]
                        .AsArray()
                        .Select(x => (float)x.AsValue())
                        .ToArray();
                    arrEmbeddings.AddRange(embeddings);
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
        }

        var result = arrEmbeddings.Select(f => (new[] { f })).ToArray();

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