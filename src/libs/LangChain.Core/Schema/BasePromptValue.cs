using LangChain.Providers;

namespace LangChain.Schema;

/// <summary>
/// 
/// </summary>
public abstract class BasePromptValue
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract IReadOnlyCollection<Message> ToChatMessages();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override abstract string ToString();
}