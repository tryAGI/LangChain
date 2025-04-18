using System.CommandLine;

namespace LangChain.Cli.Commands;

internal sealed class DoCommand : Command
{
    public DoCommand() : base(name: "do", description: "Generates text using a prompt.")
    {
        var handler = new DoCommandHandler();
        
        AddOption(handler.InputOption);
        AddOption(handler.InputFileOption);
        AddOption(handler.OutputFileOption);
        AddOption(handler.ToolsOption);
        AddOption(handler.DirectoriesOption);
        AddOption(handler.FormatOption);
        AddOption(handler.DebugOption);
        AddOption(handler.ModelOption);
        
        Handler = handler;
    }
}