using System.CommandLine;

namespace LangChain.Cli.Commands.Auth;

public class AuthCommand : Command
{
    public AuthCommand() : base(name: "auth", description: "Authenticates a provider.")
    {
        AddCommand(new OpenAiCommand());
        AddCommand(new OpenRouterCommand());
    }
}