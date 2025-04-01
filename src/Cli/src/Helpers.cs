using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using OpenAI;

namespace LangChain.Cli;

internal static class Helpers
{
    public static string GetSettingsFolder()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LangChainDotnet.Cli");
        Directory.CreateDirectory(folder);

        return folder;
    }

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

    public static async Task WriteOutputAsync(string outputText, string outputPath, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(outputPath))
        {
            await File.WriteAllTextAsync(outputPath, outputText, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            Console.WriteLine(outputText);
        }
    }

    public static async Task SetModelAsync(string model)
    {
        var settingsFolder = GetSettingsFolder();

        await File.WriteAllTextAsync(Path.Combine(settingsFolder, "model.txt"), model).ConfigureAwait(false);

        Console.WriteLine($"Model set to {model}");
    }

    public static async Task AuthenticateWithApiKeyAsync(string apiKey, string model, string provider)
    {
        var settingsFolder = GetSettingsFolder();

        await File.WriteAllTextAsync(Path.Combine(settingsFolder, "provider.txt"), provider).ConfigureAwait(false);
        await File.WriteAllTextAsync(Path.Combine(settingsFolder, "api_key.txt"), apiKey).ConfigureAwait(false);
        await SetModelAsync(model).ConfigureAwait(false);
    }

    public static async Task<IChatClient> GetChatModelAsync(CancellationToken cancellationToken = default)
    {
        var settingsFolder = GetSettingsFolder();

        var provider = await File.ReadAllTextAsync(Path.Combine(settingsFolder, "provider.txt"), cancellationToken)
            .ConfigureAwait(false);
        var modelId = await File.ReadAllTextAsync(Path.Combine(settingsFolder, "model.txt"), cancellationToken).ConfigureAwait(false);
        IChatClient chatClient;
        Uri? endpoint = provider switch
        {
            Providers.OpenRouter => new Uri(tryAGI.OpenAI.CustomProviders.OpenRouterBaseUrl),
            _ => null,
        };
        modelId = modelId switch
        {
            "latest-fast" => tryAGI.OpenAI.CreateChatCompletionRequestModelExtensions.ToValueString(tryAGI.OpenAI.ChatClient.LatestFastModel),
            "latest-smart" => tryAGI.OpenAI.CreateChatCompletionRequestModelExtensions.ToValueString(tryAGI.OpenAI.ChatClient.LatestSmartModel),
            _ => modelId,
        };

        switch (provider)
        {
            case Providers.OpenAi or Providers.OpenRouter:
                {
                    var apiKey = await File.ReadAllTextAsync(Path.Combine(settingsFolder, "api_key.txt"), cancellationToken).ConfigureAwait(false);
                    var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
                    {
                        Endpoint = endpoint,
                    });

                    chatClient = openAiClient.AsChatClient(modelId);
                    break;
                }
            default:
                throw new NotSupportedException("Provider not supported.");
        }

        using var factory = LoggerFactory.Create(builder =>
            builder.AddDebug().SetMinimumLevel(LogLevel.Trace));
        var client = new ChatClientBuilder(chatClient)
            // 👇🏼 Add logging to the chat client, wrapping the function invocation client 
            .UseLogging(factory)
            // 👇🏼 Add function invocation to the chat client, wrapping the Ollama client
            .UseFunctionInvocation()
            .Build();

        return client;
    }

    public static async Task<string> GenerateUsingAuthenticatedModelAsync(string prompt, CancellationToken cancellationToken = default)
    {
        IChatClient model = await GetChatModelAsync(cancellationToken).ConfigureAwait(false);

        var response = await model.GetResponseAsync(prompt, cancellationToken: cancellationToken).ConfigureAwait(false);

        return response.Text;
    }
}