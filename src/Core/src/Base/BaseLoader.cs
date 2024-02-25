using LangChain.Docstore;
using LangChain.Splitters.Text;

namespace LangChain.Base;

/// <summary>
/// 
/// </summary>
public abstract class BaseLoader
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract List<Document> Load();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="textSplitter"></param>
    /// <returns></returns>
    public List<Document> LoadAndSplit(TextSplitter? textSplitter = null)
    {
        if (textSplitter == null)
        {
            textSplitter = new RecursiveCharacterTextSplitter();
        }
        var docs = Load();
        return textSplitter.SplitDocuments(docs);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual IEnumerable<Document> LazyLoad()
    {
        throw new NotImplementedException($"{GetType().Name} does not implement LazyLoad()");
    }
}