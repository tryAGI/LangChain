using System.CommandLine;
using OpenAI.Constants;

namespace LangChain.Cli.Commands;

public class ModelCommand : Command
{
    public ModelCommand() : base(name: "model", description: "Sets model for provider.")
    {
        var model = new Argument<string>(
            name: "Model",
            getDefaultValue: () => ChatModels.Gpt35Turbo,
            description: "Model to use for commands");
        AddArgument(model);
        
        this.SetHandler(HandleAsync, model);
    }
    
    private static async Task HandleAsync(string model)
    {
        await Helpers.SetModelAsync(model).ConfigureAwait(false);
    }
}