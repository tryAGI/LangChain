using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using LangChain.Providers.DeepInfra.CodeGenerator.Main;

var rootCommand = new RootCommand(
    "Create Codes for OpenRouter Models")
{
    new Option<bool>(
        ["--underscore", "-u"], "Add underscores into enum member name"),
    new Option<string>(
        ["--output", "-o"], getDefaultValue: () => "../../../../../src", "Output directory")
};

rootCommand.Handler = CommandHandler.Create(async (bool underscore, string output) =>
{
    var replaces = new Dictionary<string, string>
    {
        { " ", "" },
        { "+", "Plus" },
        { "BV0", "Bv0" },
        { "7b", "7B" },
        { "bBa", "BBa" },
        { "bSf", "BSf" },
        { "DPO", "Dpo" },
        { "SFT", "Sft" },
        { "Openai", "OpenAi" },
        { "Openchat", "OpenChat" },
        { "Openher", "OpenHer" },
        { "Openorca", "OpenOrca" },
    };
    if (underscore)
    {
        replaces[" "] = "_";
    }

    await DeepInfraCodeGenerator.GenerateCodesAsync(new GenerationOptions
    {
        OutputFolder = output,
        ReplaceEnumNameFunc = ReplaceFunc,
    }).ConfigureAwait(false);
    return;


    string ReplaceFunc(string enumType, string modelId, string modelName)
    {
        foreach (var (oldValue, newValue) in replaces)
        {
            enumType = enumType.Replace(oldValue, newValue, StringComparison.InvariantCulture);
        }

        if (modelId == "openai/gpt-3.5-turbo-0125")
        {
            enumType = underscore
                ? "OpenAi_Gpt_3_5_Turbo_16K_0125"
                : "OpenAiGpt35Turbo16K0125";
        }

        return enumType;
    }
});

return await rootCommand.InvokeAsync(args).ConfigureAwait(false);