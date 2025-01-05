using System.CommandLine;

namespace LangChain.Cli.Commands;

internal sealed class ModelCommand : Command
{
    public ModelCommand() : base(name: "model", description: "Sets model for provider.")
    {
        var model = new Argument<string>(
            name: "Model",
            getDefaultValue: () => "latest-fast",
            description: "Model to use for commands. You can use latest-smart or latest-fast or any specific model. Default is latest-fast.");
        AddArgument(model);

        this.SetHandler(HandleAsync, model);
    }

    private static async Task HandleAsync(string model)
    {
        await Helpers.SetModelAsync(model).ConfigureAwait(false);
    }
}