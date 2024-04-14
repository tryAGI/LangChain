using System.CommandLine;
using LangChain.Cli.Commands;
using LangChain.Cli.Commands.Auth;

var rootCommand = new RootCommand(
    description: "CLI tool to use LangChain for common tasks");
rootCommand.AddCommand(new AuthCommand());
rootCommand.AddCommand(new GenerateCommand());
rootCommand.AddCommand(new SummarizeCommand());

return await rootCommand.InvokeAsync(args).ConfigureAwait(false);