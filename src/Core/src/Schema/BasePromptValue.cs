using Microsoft.Extensions.AI;

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
    public abstract IReadOnlyCollection<ChatMessage> ToChatMessages();

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public override abstract string ToString();
}
