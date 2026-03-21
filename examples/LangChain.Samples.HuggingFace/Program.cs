using Microsoft.Extensions.AI;

var apiKey =
    Environment.GetEnvironmentVariable("HUGGINGFACE_API_KEY") ??
    throw new InvalidOperationException("HUGGINGFACE_API_KEY environment variable is not found.");

using var client = new HuggingFace.HuggingFaceInferenceClient(apiKey);
IChatClient chatClient = (IChatClient)client;

var response = await chatClient.GetResponseAsync(
    "What would be a good company name for a company that makes colorful socks?",
    new ChatOptions { ModelId = "meta-llama/Llama-3.3-70B-Instruct" });

Console.WriteLine("### HuggingFace Response");
Console.WriteLine(response.Text);
