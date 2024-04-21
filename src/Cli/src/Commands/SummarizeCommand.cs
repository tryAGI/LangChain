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
        
        AddOption(inputOption);
        AddOption(inputFileOption);
        AddOption(outputFileOption);
        AddOption(wordCountOption);
        
        this.SetHandler(HandleAsync, inputOption, inputFileOption, outputFileOption, wordCountOption);
    }
    
    private static async Task HandleAsync(string input, string inputPath, string outputPath, int wordCount)
    {
        var inputText = await Helpers.ReadInputAsync(input, inputPath).ConfigureAwait(false);
        var outputText = await Helpers.GenerateUsingAuthenticatedModelAsync(
            $"""
             Please summarize the the following text in {wordCount} words or less:
             {inputText}
             """).ConfigureAwait(false);
        
        await Helpers.WriteOutputAsync(outputText, outputPath).ConfigureAwait(false);
    }
}