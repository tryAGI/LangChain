using System.CommandLine;
using System.CommandLine.Parsing;
using LangChain.Cli.Commands;

var rootCommand = new RootCommand(
    description: "CLI tool to use LangChain for common tasks");
rootCommand.Add(new DoCommand());

var parseResult = CommandLineParser.Parse(rootCommand, args, new ParserConfiguration());
return await parseResult.InvokeAsync(new InvocationConfiguration(), CancellationToken.None).ConfigureAwait(false);
