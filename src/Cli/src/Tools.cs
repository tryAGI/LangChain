namespace LangChain.Cli;

internal static class Tools
{
    public const string Filesystem = "filesystem";
    public const string Fetch = "fetch";
    public const string GitHub = "github";
    public const string Git = "git";
    public const string Puppeteer = "puppeteer";
    public const string SequentialThinking = "sequential-thinking";
    public const string Slack = "slack";

    public static string[] All =>
    [
        Filesystem,
        Fetch,
        GitHub,
        Git,
        Puppeteer,
        SequentialThinking,
        Slack,
    ];
}