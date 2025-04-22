using System.CommandLine;
using LangChain.Cli.Models;

namespace LangChain.Cli;

internal static class CommonOptions
{
    public static Option<string> Input => new(
        aliases: ["--input", "-i"],
        getDefaultValue: () => string.Empty,
        description: "Input text");

    public static Option<FileInfo?> InputFile => new(
        aliases: ["--input-file"],
        getDefaultValue: () => null,
        description: "Input file path");

    public static Option<FileInfo?> OutputFile => new(
        aliases: ["--output-file"],
        getDefaultValue: () => null,
        description: "Output file path");

    public static Option<bool> Debug => new(
        aliases: ["--debug"],
        getDefaultValue: () => false,
        description: "Show Debug Information");

    public static Option<string> Model => new(
        aliases: ["--model"],
        getDefaultValue: () => "o3-mini",
        description: "Model to use for commands.");

    public static Option<Provider> Provider => new(
        aliases: ["--provider"],
        getDefaultValue: () => default,
        description: $"Provider to use for commands.");
}