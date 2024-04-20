using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Providers;

namespace LangChain.Chains.StackableChains.ImageGeneration;

/// <summary>
/// 
/// </summary>
public class ImageGenerationChain : BaseStackableChain
{
    private readonly ITextToImageModel _model;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="inputKey"></param>
    /// <param name="outputKey"></param>
    public ImageGenerationChain(
        ITextToImageModel model,
        string inputKey = "prompt",
        string outputKey = "image")
    {
        _model = model;
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };
    }

    /// <inheritdoc />
    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));
        
        var prompt =
            values.Value[InputKeys[0]].ToString() ??
            throw new InvalidOperationException("Input key is null");
        byte[] image = await _model.GenerateImageAsync(prompt).ConfigureAwait(false);
        values.Value[OutputKeys[0]] = image;
        return values;
    }
}
