using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Providers;

namespace LangChain.Chains.StackableChains;

/// <summary>
/// Text to speech chain
/// </summary>
public class TTSChain<T>:BaseStackableChain
{
    private readonly ITextToSpeechModel<T> _model;
    private readonly T _settings;
    private readonly string _inputKey;
    private readonly string _outputKey;


    public TTSChain(ITextToSpeechModel<T> model, 
        T settings,
        string inputKey = "text", string outputKey = "audio")
    {
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };
        _model = model;
        _settings = settings;
        _inputKey = inputKey;
        _outputKey = outputKey;

    }

    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        var text = (string)values.Value[_inputKey];
        var data = await _model.GenerateSpeechAsync(text, _settings);
        values.Value[_outputKey] = data;
        return values;
    }
}