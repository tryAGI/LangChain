using System.CommandLine;
using LangChain.Providers.OpenRouter;
using OpenAI.Constants;

namespace LangChain.Cli.Commands.Auth;

public class OpenRouterCommand : Command
{
    public OpenRouterCommand() : base(name: Providers.OpenRouter, description: "Authenticates OpenRouter provider.")
    {
        var apiKeyArgument = new Argument<string>(
            name: "Api key",
            description: "OpenRouter API key from https://openrouter.ai/keys");
        var modelOption = new Option<string>(
            aliases: ["--model", "-m"],
            getDefaultValue: () => OpenRouterModelProvider.GetModelById(OpenRouterModelIds.Mistral7BInstructFree),
            description: "Model to use for commands");
        AddArgument(apiKeyArgument);
        AddOption(modelOption);

        this.SetHandler(HandleAsync, apiKeyArgument, modelOption);
    }

    private static async Task HandleAsync(string apiKey, string model)
    {
        await Helpers.AuthenticateWithApiKeyAsync(apiKey, model, Providers.OpenRouter).ConfigureAwait(false);
    }
}