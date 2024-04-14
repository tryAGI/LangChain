using System.CommandLine;

namespace LangChain.Cli;

public static class Arguments
{
    public static Argument<string> InputPath => new(
        name: "Input file",
        description: "Input file path");
    
    public static Argument<string> OutputPath => new (
        name: "Output file",
        description: "Output file path");
}