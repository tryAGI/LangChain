using System.CommandLine;
using OpenAI;

namespace LangChain.Cli.Commands.Auth;

public class OpenAiCommand : Command
{
    public OpenAiCommand() : base(name: Providers.OpenAi, description: "Authenticates OpenAI provider.")
    {
        var apiKeyArgument = new Argument<string>(
            name: "Api key",
            description: "OpenAI API key from https://platform.openai.com/account/api-keys");
        var modelOption = new Option<string>(
            aliases: ["--model", "-m"],
            getDefaultValue: () => CreateChatCompletionRequestModel.Gpt35Turbo.ToValueString(),
            description: "Model to use for commands");
        AddArgument(apiKeyArgument);
        AddOption(modelOption);

        this.SetHandler(HandleAsync, apiKeyArgument, modelOption);
    }

    private static async Task HandleAsync(string apiKey, string model)
    {
        await Helpers.AuthenticateWithApiKeyAsync(apiKey, model, Providers.OpenAi).ConfigureAwait(false);
    }
}