using System.CommandLine;

namespace LangChain.Cli.Commands.Auth;

internal sealed class OpenAiCommand : Command
{
    public OpenAiCommand() : base(name: Providers.OpenAi, description: "Authenticates OpenAI provider.")
    {
        var apiKeyArgument = new Argument<string>(
            name: "Api key",
            description: "OpenAI API key from https://platform.openai.com/account/api-keys");
        var modelOption = new Option<string>(
            aliases: ["--model", "-m"],
            getDefaultValue: () => "latest-fast",
            description: "Model to use for commands. You can use latest-smart or latest-fast or any specific model. Default is latest-fast.");
        AddArgument(apiKeyArgument);
        AddOption(modelOption);

        this.SetHandler(HandleAsync, apiKeyArgument, modelOption);
    }

    private static async Task HandleAsync(string apiKey, string model)
    {
        await Helpers.AuthenticateWithApiKeyAsync(apiKey, model, Providers.OpenAi).ConfigureAwait(false);
    }
}