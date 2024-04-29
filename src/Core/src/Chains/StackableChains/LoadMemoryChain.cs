using LangChain.Abstractions.Schema;
using LangChain.Chains.HelperChains;
using LangChain.Memory;
using LangChain.Schema;

namespace LangChain.Chains.StackableChains;

public class LoadMemoryChain : BaseStackableChain
{
    private readonly BaseChatMemory _chatMemory;
    private readonly string _outputKey;

    public LoadMemoryChain(BaseChatMemory chatMemory, string outputKey)
    {
        _chatMemory = chatMemory;
        _outputKey = outputKey;

        OutputKeys = new[] { _outputKey };
    }

    protected override Task<IChainValues> InternalCallAsync(
        IChainValues values,
        CancellationToken cancellationToken = default)
    {
        values = values ?? throw new ArgumentNullException(nameof(values));

        var memoryVariableName = _chatMemory.MemoryVariables.FirstOrDefault();
        if (memoryVariableName == null)
        {
            throw new InvalidOperationException("Missing memory variable name");
        }

        OutputValues outputValues = _chatMemory.LoadMemoryVariables(null);
        values.Value[_outputKey] = outputValues.Value[memoryVariableName];
        return Task.FromResult(values);
    }
}