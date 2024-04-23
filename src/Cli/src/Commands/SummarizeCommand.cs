using System.CommandLine;

namespace LangChain.Cli.Commands;

public class SummarizeCommand : Command
{
    public SummarizeCommand() : base(name: "summarize", description: "Summarizes text using a provider.")
    {
        var inputOption = CommonOptions.Input;
        var inputFileOption = CommonOptions.InputFile;
        var outputFileOption = CommonOptions.OutputFile;
        var wordCountOption = new Option<int>(
            aliases: ["--word-count", "-w"],
            getDefaultValue: () => 20,
            description: "Word count for summary");
        
        var providerOption = new Option<string>(
            aliases: ["--provider", "-p"], 
            getDefaultValue: () => Providers.OpenAi,
            description: "Provider to use for summarization (default: OpenAI)");

        AddOption(inputOption);
        AddOption(inputFileOption);
        AddOption(outputFileOption);
        AddOption(wordCountOption);
        AddOption(providerOption);
        
        this.SetHandler(HandleAsync, inputOption, inputFileOption, outputFileOption, wordCountOption, providerOption);
    }
    
    private static async Task HandleAsync(string input, string inputPath, string outputPath, int wordCount, string provider)
    {
        var inputText = await Helpers.ReadInputAsync(input, inputPath).ConfigureAwait(false);
        string outputText;
    
        if (provider == Providers.TogetherAi)
        {
            var togetherAiProvider = new TogetherAiProvider();
            var model = new TogetherAiModel(togetherAiProvider, TogetherAiModelIds.TogetherAiGpt3_5);
            
            outputText = await model.GenerateAsync(
                $"""
                 Please summarize the the following text in {wordCount} words or less:
                 {inputText}
                 """);
        }
        else
        {
            outputText = await Helpers.GenerateUsingAuthenticatedModelAsync(
                $"""
                 Please summarize the the following text in {wordCount} words or less:
                 {inputText}
                 """).ConfigureAwait(false);
        }
        
        await Helpers.WriteOutputAsync(outputText, outputPath).ConfigureAwait(false);
    }
}