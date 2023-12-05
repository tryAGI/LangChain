using LangChain.Schema;

namespace LangChain.Memory;

/// <summary>
/// 
/// </summary>
public abstract class BaseMemory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputValues"></param>
    /// <returns></returns>
    public abstract OutputValues LoadMemoryVariables(InputValues? inputValues);
    
    /// <summary>
    /// 
    /// </summary>
    public abstract List<string> MemoryVariables { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputValues"></param>
    /// <param name="outputValues"></param>
    /// <returns></returns>
    public abstract Task SaveContext(InputValues inputValues, OutputValues outputValues);
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract Task Clear();

}