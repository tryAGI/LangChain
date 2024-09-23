using LangChain.Providers;
using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenRouter;
using OpenAI;

namespace LangChain.Cli;

public static class Helpers
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
            inputText = await File.ReadAllTextAsync(inputPath, cancellationToken).ConfigureAwait(false);
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

    public static async Task<ChatModel> GetChatModelAsync(CancellationToken cancellationToken = default)
    {
        var settingsFolder = GetSettingsFolder();

        ChatModel model;
        switch (await File.ReadAllTextAsync(Path.Combine(settingsFolder, "provider.txt"), cancellationToken).ConfigureAwait(false))
        {
            case Providers.OpenAi:
                {
                    var provider = new OpenAiProvider(apiKey: await File.ReadAllTextAsync(Path.Combine(settingsFolder, "api_key.txt"), cancellationToken).ConfigureAwait(false));
                    var modelId = await File.ReadAllTextAsync(Path.Combine(settingsFolder, "model.txt"), cancellationToken).ConfigureAwait(false);
                    switch (modelId)
                    {
                        case "latest-fast":
                            modelId = ChatClient.LatestFastModel.ToValueString();
                            break;
                        case "latest-smart":
                            modelId = ChatClient.LatestSmartModel.ToValueString();
                            break;
                    }

                    model = new OpenAiChatModel(provider, id: modelId);
                    break;

                }
            case Providers.OpenRouter:
                {
                    var provider = new OpenRouterProvider(apiKey: await File.ReadAllTextAsync(Path.Combine(settingsFolder, "api_key.txt"), cancellationToken).ConfigureAwait(false));
                    var modelId = await File.ReadAllTextAsync(Path.Combine(settingsFolder, "model.txt"), cancellationToken).ConfigureAwait(false);
                    model = new OpenRouterModel(provider, id: modelId);
                    break;
                }
            default:
                throw new NotSupportedException("Provider not supported.");
        }

        return model;
    }

    public static async Task<string> GenerateUsingAuthenticatedModelAsync(string prompt, CancellationToken cancellationToken = default)
    {
        ChatModel model = await GetChatModelAsync(cancellationToken).ConfigureAwait(false);

        return await model.GenerateAsync(prompt, cancellationToken: cancellationToken);
    }
}