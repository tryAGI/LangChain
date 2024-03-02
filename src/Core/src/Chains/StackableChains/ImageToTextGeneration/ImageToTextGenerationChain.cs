using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Providers;

namespace LangChain.Chains.StackableChains.ImageToTextGeneration;

/// <summary>
/// 
/// </summary>
public class ImageToTextGenerationChain : BaseStackableChain
{
    private readonly IImageToTextModel _model;
    private readonly BinaryData _image;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="image"></param>
    /// <param name="outputKey"></param>
    public ImageToTextGenerationChain(
        IImageToTextModel model,
        BinaryData image,
        string outputKey = "text")
    {
        _model = model;
        _image = image;
        OutputKeys = new[] { outputKey };
    }

    /// <inheritdoc />
    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        var text = await _model.GenerateTextFromImageAsync(new ImageToTextRequest { Image = _image }).ConfigureAwait(false);
        values.Value[OutputKeys[0]] = text;
        return values;
    }
}
