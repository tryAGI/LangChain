using Microsoft.Extensions.AI;
using OpenAI;

var apiKey =
    Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
    throw new InvalidOperationException("OPENAI_API_KEY environment variable is not found.");

IChatClient chatClient = new OpenAIClient(apiKey).GetChatClient("gpt-4o-mini").AsIChatClient();

var result = await chatClient.GetResponseAsync("What is a good name for a company that sells colourful socks?");

Console.WriteLine(result.Text);
