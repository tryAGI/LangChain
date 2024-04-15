using System.CommandLine;

namespace LangChain.Cli.Commands;

public class SummarizeCommand : Command
{
    public SummarizeCommand() : base(name: "summarize", description: "Summarizes text using a provider.")
    {
        var inputPathArgument = CommonArguments.InputPath;
        var outputPathArgument = CommonArguments.OutputPath;
        var wordCountOption = new Option<int>(
            aliases: ["--word-count", "-w"], getDefaultValue: () => 20, description: "Word count for summary");
        
        AddArgument(inputPathArgument);
        AddArgument(outputPathArgument);
        AddOption(wordCountOption);
        
        this.SetHandler(HandleAsync, inputPathArgument, outputPathArgument, wordCountOption);
    }
    
    private static async Task HandleAsync(string inputPath, string outputPath, int wordCount)
    {
        var inputText = await File.ReadAllTextAsync(inputPath).ConfigureAwait(false);
    
        var outputText = await Helpers.GenerateUsingAuthenticatedModelAsync(
            $"""
             Please summarize the the following text in {wordCount} words or less:
             {inputText}
             """).ConfigureAwait(false);
            
        await File.WriteAllTextAsync(outputPath, outputText).ConfigureAwait(false);
    }
}