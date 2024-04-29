namespace LangChain.Databases.OpenSearch;

public class OpenSearchVectorDatabaseOptions
{
    public Uri? ConnectionUri { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}