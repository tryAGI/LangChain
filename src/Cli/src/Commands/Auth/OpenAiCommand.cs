using System.CommandLine;
using OpenAI.Constants;

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
            getDefaultValue: () => ChatModels.Gpt35Turbo,
            description: "Model to use for commands");
        var temperatureOption = new Option<double>(
            aliases: ["--temperature", "-t"], 
            getDefaultValue: () => 0.7,
            description: "Sampling temperature to use (between 0 and 2)");
        AddArgument(apiKeyArgument);
        AddOption(modelOption);
        AddOption(temperatureOption);
        
        this.SetHandler(HandleAsync, apiKeyArgument, modelOption, temperatureOption);
    }
    
    private static async Task HandleAsync(string apiKey, string model, double temperature)
    {
        await Helpers.AuthenticateWithApiKeyAsync(apiKey, model, Providers.OpenAi, temperature).ConfigureAwait(false);
    }
}