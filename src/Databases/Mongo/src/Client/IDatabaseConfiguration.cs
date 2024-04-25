namespace LangChain.Databases.Mongo.Client;

public interface IDatabaseConfiguration
{
    string ConnectionString { get; set; }

    string DatabaseName { get; set; }
}