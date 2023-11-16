using LangChain.Schema;

namespace LangChain.Memory;

public abstract class BaseMemory
{
    public abstract OutputValues LoadMemoryVariables(InputValues inputValues);
    public abstract List<string> MemoryVariables { get; }
    public abstract Task SaveContext(InputValues inputValues, OutputValues outputValues);
    public abstract Task Clear();

}