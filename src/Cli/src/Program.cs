using System.CommandLine;
using LangChain.Cli.Commands;

var rootCommand = new RootCommand(
    description: "CLI tool to use LangChain for common tasks");
rootCommand.AddCommand(new DoCommand());

return await rootCommand.InvokeAsync(args).ConfigureAwait(false);