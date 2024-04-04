using LangChain.Providers.OpenRouter.CodeGenerator.Main;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Help;

RootCommand rootCommand = new RootCommand(
    description: "Create Codes for OpenRouter Models")
{
    new Option<bool>(
        aliases: ["--underscore", "-u"]
        , description: "Add underscores into enum member name"),
    new Option<string?>(
    aliases: [ "--output", "-o" ]
};

rootCommand.Handler = CommandHandler.Create(async (bool underscore, string? output) =>
{
   
    var gc = new OpenRouterCodeGenerator();
    await gc.GenerateCodesAsync(false,output).ConfigureAwait(false);
});

return await rootCommand.InvokeAsync(args);



