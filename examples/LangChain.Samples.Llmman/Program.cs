using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;

// llmman (https://github.com/llmmanorg/llmman) serves an OpenAI-compatible API on port 17434.
// Setup: llmman pull gemma4 && llmman serve. LLMMAN_HOST ([host][:port]) overrides the default.
var llmmanEndpoint = new UriBuilder("http://127.0.0.1:17434/v1");
if (Environment.GetEnvironmentVariable("LLMMAN_HOST") is { Length: > 0 } llmmanHost)
{
    var parts = llmmanHost.Split(':');
    if (parts[0].Length > 0) llmmanEndpoint.Host = parts[0];
    if (parts.Length > 1) llmmanEndpoint.Port = int.Parse(parts[1]);
}

// llmman needs no API key; the OpenAI client requires a placeholder.
IChatClient chatClient = new OpenAIClient(
    new ApiKeyCredential("llmman"),
    new OpenAIClientOptions { Endpoint = llmmanEndpoint.Uri })
    .GetChatClient("gemma4").AsIChatClient();

var result = await chatClient.GetResponseAsync("What is a good name for a company that sells colourful socks?");

Console.WriteLine(result.Text);
