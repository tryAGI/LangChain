using System.CommandLine;
using LangChain.Providers.OpenAI;
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
        var temperatureOption = new Option<double?>(
            aliases: ["--temperature"],
            description: "What sampling temperature to use, between 0 and 2.");
        var maxTokensOption = new Option<int?>(
            aliases: ["--max-tokens"], 
            description: "The maximum number of tokens to generate in the chat completion.");
        var topPOption = new Option<double?>(
            aliases: ["--top-p"],
            description: "An alternative to sampling with temperature, called nucleus sampling.");
        var frequencyPenaltyOption = new Option<double?>(
            aliases: ["--frequency-penalty"],
            description: "Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far.");
        var presencePenaltyOption = new Option<double?>(
            aliases: ["--presence-penalty"], 
            description: "Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far.");

        AddArgument(apiKeyArgument);
        AddOption(modelOption);
        AddOption(temperatureOption);
        AddOption(maxTokensOption); 
        AddOption(topPOption);
        AddOption(frequencyPenaltyOption);
        AddOption(presencePenaltyOption);
        
        this.SetHandler(HandleAsync, apiKeyArgument, modelOption, temperatureOption, maxTokensOption, topPOption, frequencyPenaltyOption, presencePenaltyOption);
    }
    
    private static async Task HandleAsync(
        string apiKey, 
        string model,
        double? temperature,
        int? maxTokens,
        double? topP,
        double? frequencyPenalty, 
        double? presencePenalty)
    {
        var chatSettings = new OpenAiChatSettings
        {
            Temperature = temperature,
            MaxTokens = maxTokens,
            TopP = topP,
            FrequencyPenalty = frequencyPenalty,
            PresencePenalty = presencePenalty
        };

        await Helpers.AuthenticateWithApiKeyAsync(apiKey, model, Providers.OpenAi, chatSettings).ConfigureAwait(false);
    }
}