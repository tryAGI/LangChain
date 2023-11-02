using LangChain.Docstore;
using LangChain.TextSplitters;

namespace LangChain.Base;

public abstract class BaseLoader
{
    public abstract List<Document> Load();

    public List<Document> LoadAndSplit(TextSplitter textSplitter = null)
    {
        if (textSplitter == null)
        {
            textSplitter = new RecursiveCharacterTextSplitter();
        }
        var docs = Load();
        return textSplitter.SplitDocuments(docs);
    }

    public virtual IEnumerable<Document> LazyLoad()
    {
        throw new NotImplementedException($"{GetType().Name} does not implement LazyLoad()");
    }

}