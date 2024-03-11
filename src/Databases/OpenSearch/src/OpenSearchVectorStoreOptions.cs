using OpenSearch.Client;

namespace LangChain.Databases.OpenSearch;

public class OpenSearchVectorStoreOptions
{
    //public string NodeAddress { get; set; } = "vectors.db";
    //public string IndexName { get; set; } = "vectors";
    public int ChunkSize { get; set; } = 200;
    public int ChunkOverlap { get; set; } = 50;
    public int? ShardCount { get; set; }
    public int? ReplicaCount { get; set; }
    public string? IndexName { get; set; }
    public Uri? ConnectionUri { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}