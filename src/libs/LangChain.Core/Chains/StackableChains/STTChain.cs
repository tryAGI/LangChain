using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Providers;

namespace LangChain.Chains.StackableChains;

/// <summary>
/// Speech to text chain
/// </summary>
public class STTChain<T> : BaseStackableChain
{
    private readonly ISpeechToTextModel<T> _model;
    private readonly T _settings;
    private readonly string _inputKey;
    private readonly string _outputKey;

    public STTChain(ISpeechToTextModel<T> model, T settings,
        string inputKey = "audio", string outputKey = "text")
    {
        _model = model;
        _settings = settings;
        _inputKey = inputKey;
        _outputKey = outputKey;
        InputKeys = new[] { inputKey };
        OutputKeys = new[] { outputKey };

    }

    protected override async Task<IChainValues> InternalCall(IChainValues values)
    {
        var audio = (byte[])values.Value[_inputKey];
        var text=await _model.TranscribeAsync(audio, _settings);
        values.Value[_outputKey] = text;
        return values;
    }
}