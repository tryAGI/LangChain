namespace LangChain.Databases.OpenSearch;

public class OpenSearchVectorStoreOptions
{
    public string? IndexName { get; set; }
    public Uri? ConnectionUri { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int? Dimensions { get; set; }
}