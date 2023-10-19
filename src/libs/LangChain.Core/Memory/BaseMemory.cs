using LangChain.Schema;

namespace LangChain.Memory;

public abstract class BaseMemory
{
    public abstract OutputValues LoadMemoryVariables(InputValues inputValues);
    public abstract void SaveContext(InputValues inputValues, OutputValues outputValues);
}