using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Providers;

namespace LangChain.Chains.StackableChains.ImageGeneration;

public class ImageGenerationChain : BaseStackableChain
{
    private readonly IGenerateImageModel _model;

    public ImageGenerationChain(IGenerateImageModel model, string inputKey = "prompt", string outputKey = "image")
    {
        _model = model;
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };

    }

    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        var prompt = values.Value[InputKeys[0]].ToString();
        var image = await _model.GenerateImageAsBytesAsync(prompt);
        values.Value[OutputKeys[0]] = image;
        return values;
    }
}