using LangChain.Schema;

namespace LangChain.Memory;

/// <summary>
/// Abstract base class for memory in Chains.
/// 
/// Memory refers to state in Chains.  Memory can be used to store information about
/// past executions of a Chain and inject that information into the inputs of
/// future executions of the Chain.  For example, for conversational Chains Memory
/// can be used to store conversations and automatically add them to future model
/// prompts so that the model has the necessary context to respond coherently to
/// the latest input.
/// </summary>
public abstract class BaseMemory
{
    /// <summary>
    /// Return key-value pairs given the text input to the chain.
    /// </summary>
    public abstract List<string> MemoryVariables { get; }

    /// <summary>
    /// The string keys this memory class will add to chain inputs.
    /// </summary>
    /// <param name="inputValues"></param>
    /// <returns></returns>
    public abstract OutputValues LoadMemoryVariables(InputValues? inputValues);

    /// <summary>
    /// Save the context of this chain run to memory.
    /// </summary>
    /// <param name="inputValues"></param>
    /// <param name="outputValues"></param>
    public abstract Task SaveContext(InputValues inputValues, OutputValues outputValues);

    /// <summary>
    /// Clear memory contents.
    /// </summary>
    public abstract Task Clear();

}