namespace LangChain.Base.Tracers;

public class ConsoleCallbackHandler() :
    FunctionCallbackHandler(
        new FunctionCallbackHandlerInput
        {
            Function = Print
        })
{
    public override string Name => "console_callback_handler";

    private static Task Print(string text)
    {
        Console.WriteLine(text);
        return Task.CompletedTask;
    }
}