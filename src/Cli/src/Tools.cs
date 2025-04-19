namespace LangChain.Cli;

internal static class Tools
{
    public const string Filesystem = "filesystem";
    public const string Fetch = "fetch";
    public const string GitHub = "github";
    public const string Git = "git";
    
    public static string[] All =>
    [
        Filesystem,
        Fetch,
        GitHub,
        Git
    ];
}