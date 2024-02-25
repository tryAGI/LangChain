namespace LangChain.Splitters.Text;

/// <summary>
/// 
/// </summary>
public interface ITextSplitter
{
    /// <summary>
    /// Divides a chunk of text into smaller chunks.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public IReadOnlyList<string> SplitText(string text);
}