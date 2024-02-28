﻿using System.Text;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;

namespace LangChain.Providers.Amazon.Bedrock;

internal record BedrockModelStreamRequest
{
    public static InvokeModelWithResponseStreamRequest Create(string modelId, JsonObject bodyJson)
    {
        bodyJson = bodyJson ?? throw new ArgumentNullException(nameof(bodyJson));
        
        var byteArray = Encoding.UTF8.GetBytes(bodyJson.ToJsonString());
        var stream = new MemoryStream(byteArray);

        var bedrockRequest = new InvokeModelWithResponseStreamRequest
        {
            ModelId = modelId,
            ContentType = "application/json",
            Accept = "application/json",
            Body = stream
        };

        return bedrockRequest;
    }
}