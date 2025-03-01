using System.CommandLine;
using System.Globalization;

namespace LangChain.Cli.Commands;

internal sealed class SummarizeCommand : Command
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
        var promptOption = new Option<string>(
            aliases: ["--prompt", "-p"],
            getDefaultValue: () => "Please summarize the the following text in {wordCount} words or less",
            description: "Prompt for summarization");

        AddOption(inputOption);
        AddOption(inputFileOption);
        AddOption(outputFileOption);
        AddOption(wordCountOption);

        this.SetHandler(HandleAsync, inputOption, inputFileOption, outputFileOption, promptOption, wordCountOption);
    }

    private static async Task HandleAsync(string input, string inputPath, string outputPath, string prompt, int wordCount)
    {
        var inputText = await Helpers.ReadInputAsync(input, inputPath).ConfigureAwait(false);
        var outputText = await Helpers.GenerateUsingAuthenticatedModelAsync(
            $"""
             {prompt
                 .Replace("{wordCount}", wordCount.ToString(CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase)}:
             ```
             {inputText}
             ```
             """).ConfigureAwait(false);

        await Helpers.WriteOutputAsync(outputText, outputPath).ConfigureAwait(false);
    }
}