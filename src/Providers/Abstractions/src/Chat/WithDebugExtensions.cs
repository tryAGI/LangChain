namespace LangChain.Providers;

public static class WithDebugExtensions
{
    public static T UseConsoleForDebug<T>(this T model) where T : IChatModel
    {
        model.PromptSent += (_, args) =>
        {
            Console.Write(args);
        };
        model.PartialResponseGenerated += (_, args) =>
        {
            Console.Write(args);
        };
        
        return model;
    }
}