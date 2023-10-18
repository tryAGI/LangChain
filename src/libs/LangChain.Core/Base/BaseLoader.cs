using LangChain.Docstore;

namespace LangChain.Base;

public abstract class BaseLoader
{
    public abstract List<Document> Load();

    public List<Document> LoadAndSplit(TextSplitter textSplitter=null)
    {
        throw new NotImplementedException();
    }
}