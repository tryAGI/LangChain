using System.ClientModel;
using System.CommandLine;
using System.CommandLine.IO;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using OpenAI;

namespace LangChain.Cli;

internal static class Helpers
{
    public static async Task<string> ReadInputAsync(string input, string inputPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input) && string.IsNullOrWhiteSpace(inputPath))
        {
            throw new ArgumentException("Either input or input file must be provided.");
        }

        var inputText = input;
        if (!string.IsNullOrWhiteSpace(inputPath))
        {
            if (!string.IsNullOrWhiteSpace(inputText))
            {
                inputText += Environment.NewLine;
            }

            inputText += await File.ReadAllTextAsync(inputPath, cancellationToken).ConfigureAwait(false);
        }

        return inputText;
    }

    public static async Task WriteOutputAsync(string outputText, string? outputPath, IConsole? console = null, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(outputPath))
        {
            await File.WriteAllTextAsync(outputPath, outputText, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            if (console is not null)
            {
                console.Out.WriteLine(outputText);
            }
            else
            {
                Console.WriteLine(outputText);
            }
        }
    }

    public static IChatClient GetChatModel(
        string? model = null,
        string? provider = null,
        bool debug = false)
    {
        if (debug)
        {
            Console.WriteLine("Using provider: " + provider);
            Console.WriteLine("Using model: " + model);
        }

        IChatClient chatClient;
        Uri? endpoint = provider switch
        {
            Providers.Free or Providers.OpenRouter => new Uri(tryAGI.OpenAI.CustomProviders.OpenRouterBaseUrl),
            _ => null,
        };
        model = model switch
        {
            null => "o4-mini",
            "free" or "free-fast" => "google/gemini-2.0-flash-exp:free",
            "free-smart" => "deepseek/deepseek-r1:free",
            "latest-fast" => "o4-mini",
            "latest-smart" => "o3",
            _ => model,
        };
        var apiKey = provider switch
        {
            Providers.OpenAi or null => Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set."),
            Providers.OpenRouter or Providers.Free => Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ??
                throw new InvalidOperationException("OPENROUTER_API_KEY environment variable is not set."),
            _ => throw new NotImplementedException(),
        };

        switch (provider)
        {
            default:
                {
                    var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
                    {
                        Endpoint = endpoint,
                    });

                    chatClient = openAiClient.AsChatClient(model);
                    break;
                }
        }

        using var factory = LoggerFactory.Create(builder =>
        {
            if (debug)
            {
                builder.AddConsole().SetMinimumLevel(LogLevel.Trace);
            }
            else
            {
                builder.AddDebug().SetMinimumLevel(LogLevel.Trace);
            }
        });
        var client = new ChatClientBuilder(chatClient)
            // 👇🏼 Add logging to the chat client, wrapping the function invocation client 
            .UseLogging(factory)
            // 👇🏼 Add function invocation to the chat client, wrapping the Ollama client
            .UseFunctionInvocation()
            .Build();

        return client;
    }
}