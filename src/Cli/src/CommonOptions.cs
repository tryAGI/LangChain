using System.CommandLine;
using LangChain.Cli.Models;

namespace LangChain.Cli;

internal static class CommonOptions
{
    public static Option<string> Input => new("--input", "-i")
    {
        Description = "Input text",
        DefaultValueFactory = _ => string.Empty,
    };

    public static Option<FileInfo?> InputFile => new("--input-file")
    {
        Description = "Input file path",
        DefaultValueFactory = _ => null,
    };

    public static Option<FileInfo?> OutputFile => new("--output-file")
    {
        Description = "Output file path",
        DefaultValueFactory = _ => null,
    };

    public static Option<bool> Debug => new("--debug")
    {
        Description = "Show Debug Information",
        DefaultValueFactory = _ => false,
    };

    public static Option<string> Model => new("--model")
    {
        Description = "Model to use for commands.",
        DefaultValueFactory = _ => "o3-mini",
    };

    public static Option<Provider> Provider => new("--provider")
    {
        Description = "Provider to use for commands.",
        DefaultValueFactory = _ => default,
    };
}
