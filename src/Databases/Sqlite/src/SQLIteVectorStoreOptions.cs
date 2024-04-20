namespace LangChain.Databases;

public class SQLIteVectorStoreOptions
{
    public string Filename { get; set; } = "vectors.db";
    public string TableName { get; set; } = "vectors";
    public int ChunkSize { get; set; } = 200;
    public int ChunkOverlap { get; set; } = 50;
}