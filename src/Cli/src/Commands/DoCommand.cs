using System.CommandLine;
using LangChain.Providers;

namespace LangChain.Cli.Commands;

internal sealed class DoCommand : Command
{
    public DoCommand() : base(name: "do", description: "Generates text using a prompt.")
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
        var llm = await Helpers.GetChatModelAsync().ConfigureAwait(false);
        llm.RequestSent += (_, request) => Console.WriteLine($"RequestSent: {request.Messages.AsHistory()}");
        llm.ResponseReceived += (_, response) => Console.WriteLine($"ResponseReceived: {response}");
        
        var fileSystemService = new FileSystemService();
        llm.AddGlobalTools(fileSystemService.AsTools(), fileSystemService.AsCalls());
        
        var response = await llm.GenerateAsync(inputText);
        
        await Helpers.WriteOutputAsync(response, outputPath).ConfigureAwait(false);
    }
}