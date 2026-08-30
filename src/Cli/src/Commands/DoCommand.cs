using System.CommandLine;

namespace LangChain.Cli.Commands;

internal sealed class DoCommand : Command
{
    public DoCommand() : base(name: "do", description: "Generates text using a prompt.")
    {
        var handler = new DoCommandHandler();

        Add(handler.InputOption);
        Add(handler.InputFileOption);
        Add(handler.OutputFileOption);
        Add(handler.ToolsOption);
        Add(handler.DirectoriesOption);
        Add(handler.FormatOption);
        Add(handler.DebugOption);
        Add(handler.ModelOption);
        Add(handler.ProviderOption);

        Action = handler;
    }
}
