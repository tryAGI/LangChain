using System.CommandLine;

namespace LangChain.Cli.Commands;

public class GenerateCommand : Command
{
    public GenerateCommand() : base(name: "generate", description: "Generates text using a prompt.")
    {
        var inputOption = CommonOptions.Input;
        var inputFileOption = CommonOptions.InputFile;
        var outputFileOption = CommonOptions.OutputFile;
        AddOption(inputOption);
        AddOption(inputFileOption);
        AddOption(outputFileOption);
        
        this.SetHandler(HandleAsync, inputOption, inputFileOption, outputFileOption);
    }
    
    private static async Task HandleAsync(string input, string inputPath, string outputPath)
    {
        var inputText = await Helpers.ReadInputAsync(input, inputPath).ConfigureAwait(false);
        var outputText = await Helpers.GenerateUsingAuthenticatedModelAsync(inputText).ConfigureAwait(false);

        await Helpers.WriteOutputAsync(outputText, outputPath).ConfigureAwait(false);
    }
}