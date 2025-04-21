namespace LangChain.Cli;

internal static class Formats
{
    public const string Text = "string";
    public const string Lines = "string[]";
    public const string Json = "json";
    public const string Markdown = "markdown";
    public const string ConventionalCommit = "conventional-commit";

    public static string[] All =>
    [
        Text,
        Lines,
        Json,
        Markdown,
        ConventionalCommit
    ];
}