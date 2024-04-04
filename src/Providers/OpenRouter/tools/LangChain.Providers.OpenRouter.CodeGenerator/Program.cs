using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using LangChain.Providers.OpenRouter.CodeGenerator.Main;

var rootCommand = new RootCommand(
    "Create Codes for OpenRouter Models")
{
    new Option<bool>(
        ["--underscore", "-u"]
        , "Add underscores into enum member name"),
    new Option<string?>(
        ["--output", "-o"])
};

rootCommand.Handler = CommandHandler.Create(async (bool underscore, string? output) =>
{
    var gc = new OpenRouterCodeGenerator();
    await gc.GenerateCodesAsync(false, output).ConfigureAwait(false);
});

return await rootCommand.InvokeAsync(args);