using System.CommandLine;

namespace LangChain.Cli;

internal static class CommonOptions
{
    public static Option<string> Input => new(
        aliases: ["--input", "-i"],
        getDefaultValue: () => string.Empty,
        description: "Input text");

    public static Option<string> InputFile => new(
        aliases: ["--input-file"],
        getDefaultValue: () => string.Empty,
        description: "Input file path");

    public static Option<string> OutputFile => new(
        aliases: ["--output-file"],
        getDefaultValue: () => string.Empty,
        description: "Output file path");

    public static Option<bool> Debug => new(
        aliases: ["--debug"],
        getDefaultValue: () => false,
        description: "Show Debug Information");
    
    public static Option<string> Model => new(
        aliases: ["--model"],
        getDefaultValue: () => "o3-mini",
        description: "Model to use for commands. Default is o3-mini.");
}