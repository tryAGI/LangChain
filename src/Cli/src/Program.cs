using System.CommandLine;
using LangChain.Providers.OpenAI;
using OpenAI.Constants;

var settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LangChainDotnet.Cli");
Directory.CreateDirectory(settingsFolder);

var rootCommand = new RootCommand(
    description: "CLI tool to use LangChain for common tasks");
{
    var authCommand = new Command("auth", "Authenticates a provider.");
    {
        var apiKeyArgument = new Argument<string>(
            name: "Api key", description: "OpenAI API key from https://platform.openai.com/account/api-keys");
        var modelOption = new Option<string>(
            aliases: ["--model", "-m"], getDefaultValue: () => ChatModels.Gpt35Turbo, description: "Model to use for summarization");
        var openAiCommand = new Command("openai", "Authenticates OpenAI LangChain provider.")
        {
            apiKeyArgument,
            modelOption,
        };
        openAiCommand.SetHandler(AuthenticateOpenAiAsync, apiKeyArgument, modelOption);
        authCommand.AddCommand(openAiCommand);
    }

    rootCommand.AddCommand(authCommand);
}
{
    var inputPathArgument = new Argument<string>(
        name: "Input file", description: "Input file path");
    var outputPathArgument = new Argument<string>(
        name: "Output file", description: "Output file path");
    var wordCountOption = new Option<int>(
        aliases: ["--word-count", "-w"], getDefaultValue: () => 20, description: "Word count for summary");
    var summarizeCommand = new Command("summarize", "Summarizes text using a provider.")
    {
        inputPathArgument,
        outputPathArgument,
        wordCountOption,
    };
    summarizeCommand.SetHandler(SummarizeAsync, inputPathArgument, outputPathArgument, wordCountOption);

    rootCommand.AddCommand(summarizeCommand);
}

return await rootCommand.InvokeAsync(args).ConfigureAwait(false);

// Handlers
async Task AuthenticateOpenAiAsync(string apiKey, string model)
{
    await File.WriteAllTextAsync(Path.Combine(settingsFolder, "provider.txt"), "openai").ConfigureAwait(false);
    await File.WriteAllTextAsync(Path.Combine(settingsFolder, "model.txt"), model).ConfigureAwait(false);
    await File.WriteAllTextAsync(Path.Combine(settingsFolder, "api_key.txt"), apiKey).ConfigureAwait(false);
}

async Task SummarizeAsync(string inputPath, string outputPath, int wordCount)
{
    var inputText = await File.ReadAllTextAsync(inputPath).ConfigureAwait(false);
    
    var provider = new OpenAiProvider(apiKey: await File.ReadAllTextAsync(Path.Combine(settingsFolder, "api_key.txt")).ConfigureAwait(false));
    var model = new OpenAiChatModel(provider, id: await File.ReadAllTextAsync(Path.Combine(settingsFolder, "model.txt")).ConfigureAwait(false));
    string outputText = await model.GenerateAsync(
        $"""
         Please summarize the the following text in {wordCount} words or less:
         {inputText}
         """, new OpenAiChatSettings
        {
            MaxTokens = 400,
        }).ConfigureAwait(false);
            
    await File.WriteAllTextAsync(outputPath, outputText).ConfigureAwait(false);
}