using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;

// Azure OpenAI via the OpenAI SDK (supports Azure endpoints natively)
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "https://your-resource.openai.azure.com/";
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "YOUR_API_KEY";
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT") ?? "gpt-4o-mini";

var openAiClient = new OpenAIClient(
    new ApiKeyCredential(apiKey),
    new OpenAIClientOptions { Endpoint = new Uri(endpoint) });

IChatClient chatClient = openAiClient.GetChatClient(deploymentName).AsIChatClient();

var result = await chatClient.GetResponseAsync("What is a good name for a company that sells colourful socks?");

Console.WriteLine(result.Text);
