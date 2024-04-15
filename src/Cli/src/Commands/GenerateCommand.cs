using System.CommandLine;

namespace LangChain.Cli.Commands;

public class GenerateCommand : Command
{
    public GenerateCommand() : base(name: "generate", description: "Generates text using a prompt.")
    {
        var inputPathArgument = CommonArguments.InputPath;
        var outputPathArgument = CommonArguments.OutputPath;
        AddArgument(inputPathArgument);
        AddArgument(outputPathArgument);
        
        this.SetHandler(HandleAsync, inputPathArgument, outputPathArgument);
    }
    
    private static async Task HandleAsync(string inputPath, string outputPath)
    {
        var inputText = await File.ReadAllTextAsync(inputPath).ConfigureAwait(false);
    
        var outputText = await Helpers.GenerateUsingAuthenticatedModelAsync(inputText).ConfigureAwait(false);
            
        await File.WriteAllTextAsync(outputPath, outputText).ConfigureAwait(false);
    }
}