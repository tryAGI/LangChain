namespace LangChain.Cli;

internal static class Tools
{
    public const string Filesystem = "filesystem";
    public const string Fetch = "fetch";
    public const string GitHub = "github";
    
    public static string[] All =>
    [
        Filesystem,
        Fetch,
        GitHub
    ];
}