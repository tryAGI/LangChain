using LangChain.Providers;
using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenRouter;

namespace LangChain.Cli;

public static class Helpers
{
    public static string GetSettingsFolder()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LangChainDotnet.Cli");
        Directory.CreateDirectory(folder);

        return folder;
    }
    
    public static async Task AuthenticateWithApiKeyAsync(string apiKey, string model, string provider)
    {
        var settingsFolder = GetSettingsFolder();
        
        await File.WriteAllTextAsync(Path.Combine(settingsFolder, "provider.txt"), provider).ConfigureAwait(false);
        await File.WriteAllTextAsync(Path.Combine(settingsFolder, "model.txt"), model).ConfigureAwait(false);
        await File.WriteAllTextAsync(Path.Combine(settingsFolder, "api_key.txt"), apiKey).ConfigureAwait(false);
    }

    public static async Task<string> GenerateUsingAuthenticatedModelAsync(string prompt)
    {
        var settingsFolder = GetSettingsFolder();

        ChatModel model;
        switch (await File.ReadAllTextAsync(Path.Combine(settingsFolder, "provider.txt")).ConfigureAwait(false))
        {
            case Providers.OpenAi:
            {
                var provider = new OpenAiProvider(apiKey: await File.ReadAllTextAsync(Path.Combine(settingsFolder, "api_key.txt")).ConfigureAwait(false));
                model = new OpenAiChatModel(provider, id: await File.ReadAllTextAsync(Path.Combine(settingsFolder, "model.txt")).ConfigureAwait(false));
                break;
            
            }
            case Providers.OpenRouter:
            {
                var provider = new OpenRouterProvider(apiKey: await File.ReadAllTextAsync(Path.Combine(settingsFolder, "api_key.txt")).ConfigureAwait(false));
                model = new OpenRouterModel(provider, id: await File.ReadAllTextAsync(Path.Combine(settingsFolder, "model.txt")).ConfigureAwait(false));
                break;
            }
            default:
                throw new NotSupportedException("Provider not supported.");
        }
    
        return await model.GenerateAsync(prompt).ConfigureAwait(false);
    }
}