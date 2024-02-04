namespace LangChain.Providers;

public static class WithDebugExtensions
{
    public static T UseConsoleForDebug<T>(this T model) where T : IWithDebug
    {
        model.PromptSent += Console.Write;
        model.TokenGenerated += Console.Write;
        return model;
    }
}