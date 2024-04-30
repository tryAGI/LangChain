namespace LangChain.Providers;

public static class WithDebugExtensions
{
    public static T UseConsoleForDebug<T>(this T model) where T : IChatModel
    {
        model.PromptSent += (_, text) =>
        {
            Console.Write(text);
        };
        model.PartialResponseGenerated += (_, text) =>
        {
            Console.Write(text);
        };

        return model;
    }
}