using System.CommandLine;
using LangChain.Cli;
using LangChain.Providers;
using LangChain.Providers.OpenAI;
using LangChain.Providers.OpenRouter;
using OpenAI.Constants;

var settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LangChainDotnet.Cli");
Directory.CreateDirectory(settingsFolder);

var rootCommand = new RootCommand(
    description: "CLI tool to use LangChain for common tasks");
{
    var command = new Command("auth", "Authenticates a provider.");
    {
        var apiKeyArgument = new Argument<string>(
            name: "Api key",
            description: "OpenAI API key from https://platform.openai.com/account/api-keys");
        var modelOption = new Option<string>(
            aliases: ["--model", "-m"],
            getDefaultValue: () => ChatModels.Gpt35Turbo,
            description: "Model to use for commands");
        var subCommand = new Command(Providers.OpenAi, "Authenticates OpenAI provider.")
        {
            apiKeyArgument,
            modelOption,
        };
        subCommand.SetHandler(AuthenticateOpenAiAsync, apiKeyArgument, modelOption);
        command.AddCommand(subCommand);
    }
    {
        var apiKeyArgument = new Argument<string>(
            name: "Api key",
            description: "OpenRouter API key from https://openrouter.ai/keys");
        var modelOption = new Option<string>(
            aliases: ["--model", "-m"],
            getDefaultValue: () => OpenRouterModelProvider.GetModelById(OpenRouterModelIds.Mistral7BInstructFree),
            description: "Model to use for commands");
        var subCommand = new Command(Providers.OpenRouter, "Authenticates OpenRouter provider.")
        {
            apiKeyArgument,
            modelOption,
        };
        subCommand.SetHandler(AuthenticateOpenRouterAsync, apiKeyArgument, modelOption);
        command.AddCommand(subCommand);
    }

    rootCommand.AddCommand(command);
}
{
    var inputPathArgument = Arguments.InputPath;
    var outputPathArgument = Arguments.OutputPath;
    var command = new Command("generate", "Generates text using a prompt.")
    {
        inputPathArgument,
        outputPathArgument,
    };
    command.SetHandler(GenerateAsync, inputPathArgument, outputPathArgument);

    rootCommand.AddCommand(command);
}
{
    var inputPathArgument = Arguments.InputPath;
    var outputPathArgument = Arguments.OutputPath;
    var wordCountOption = new Option<int>(
        aliases: ["--word-count", "-w"], getDefaultValue: () => 20, description: "Word count for summary");
    var command = new Command("summarize", "Summarizes text using a provider.")
    {
        inputPathArgument,
        outputPathArgument,
        wordCountOption,
    };
    command.SetHandler(SummarizeAsync, inputPathArgument, outputPathArgument, wordCountOption);

    rootCommand.AddCommand(command);
}

return await rootCommand.InvokeAsync(args).ConfigureAwait(false);

// Handlers
async Task AuthenticateOpenAiAsync(string apiKey, string model)
{
    await AuthenticateWithApiKeyAsync(apiKey, model, Providers.OpenAi).ConfigureAwait(false);
}

async Task AuthenticateOpenRouterAsync(string apiKey, string model)
{
    await AuthenticateWithApiKeyAsync(apiKey, model, Providers.OpenRouter).ConfigureAwait(false);
}

async Task SummarizeAsync(string inputPath, string outputPath, int wordCount)
{
    var inputText = await File.ReadAllTextAsync(inputPath).ConfigureAwait(false);
    
    var outputText = await GenerateUsingAuthenticatedModelAsync(
        $"""
         Please summarize the the following text in {wordCount} words or less:
         {inputText}
         """).ConfigureAwait(false);
            
    await File.WriteAllTextAsync(outputPath, outputText).ConfigureAwait(false);
}

async Task GenerateAsync(string inputPath, string outputPath)
{
    var inputText = await File.ReadAllTextAsync(inputPath).ConfigureAwait(false);
    
    var outputText = await GenerateUsingAuthenticatedModelAsync(inputText).ConfigureAwait(false);
            
    await File.WriteAllTextAsync(outputPath, outputText).ConfigureAwait(false);
}

// Helpers
async Task AuthenticateWithApiKeyAsync(string apiKey, string model, string provider)
{
    await File.WriteAllTextAsync(Path.Combine(settingsFolder, "provider.txt"), provider).ConfigureAwait(false);
    await File.WriteAllTextAsync(Path.Combine(settingsFolder, "model.txt"), model).ConfigureAwait(false);
    await File.WriteAllTextAsync(Path.Combine(settingsFolder, "api_key.txt"), apiKey).ConfigureAwait(false);
}

async Task<string> GenerateUsingAuthenticatedModelAsync(string prompt)
{
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