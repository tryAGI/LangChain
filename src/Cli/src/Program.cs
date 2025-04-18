using System.CommandLine;
using System.Text;
using LangChain.Cli.Commands;
using LangChain.Cli.Commands.Auth;

var rootCommand = new RootCommand(
    description: "CLI tool to use LangChain for common tasks");
rootCommand.AddCommand(new AuthCommand());
rootCommand.AddCommand(new GenerateCommand());
rootCommand.AddCommand(new SummarizeCommand());
rootCommand.AddCommand(new ModelCommand());
rootCommand.AddCommand(new DoCommand());

return await rootCommand.InvokeAsync(args).ConfigureAwait(false);