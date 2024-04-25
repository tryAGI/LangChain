namespace LangChain.Databases.Mongo.Client;

public class DatabaseConfiguration : IDatabaseConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;
}