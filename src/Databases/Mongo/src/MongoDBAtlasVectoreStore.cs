using LangChain.Databases.Mongo.Client;
using LangChain.Memory;
using LangChain.Databases.Mongo.Model;
using System.Text.Json;
using System.Text.Json.Serialization;
using LangChain.Providers;
using MongoDB.Driver;
using SharpCompress.Common;
using MongoDB.Bson.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.ObjectModel;

namespace LangChain.Databases.Mongo;

public class MongoDBAtlasVectorStore<T>(
    IMongoContext mongoContext,
    string collectionName,
    string indexName)
: IVectorDatabase where T : VectorSearchItem
{
    protected IMongoContext MongoContext { get; } = mongoContext;
    protected IMongoCollection<T> mongoCollection { get; } = mongoContext.GetCollection<T>(collectionName);

    public async Task<IReadOnlyCollection<string>> AddAsync(IReadOnlyCollection<VectorSearchItem> items, CancellationToken cancellationToken = default)
    {
        await mongoCollection.InsertManyAsync((IEnumerable<T>)items, cancellationToken: cancellationToken).ConfigureAwait(false);
        return items.Select(i => i.Id).ToList();
    }

    public async Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.In(i => i.Id, ids);
        var result = await mongoCollection.DeleteManyAsync(filter, cancellationToken).ConfigureAwait(false);
        return result.IsAcknowledged;
    }

    public async Task<VectorSearchResponse> SearchAsync(VectorSearchRequest request, VectorSearchSettings? settings = null, CancellationToken cancellationToken = default)
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        settings ??= new VectorSearchSettings();

        var options = new VectorSearchOptions<T>()
        {
            IndexName = indexName,
            NumberOfCandidates = settings.NumberOfResults * 10
        };
        var projectionDefinition = Builders<T>.Projection
                         .Exclude(a => a.Distance)
                         .Meta("score", "vectorSearchScore");

        var results = await mongoCollection.Aggregate()
                   .VectorSearch("Embedding", request.Embeddings.First(), settings.NumberOfResults, options)
                   .Project(projectionDefinition)
                   .ToListAsync(cancellationToken)
                   .ConfigureAwait(false);


        return new VectorSearchResponse
        {
            Items = results.Select(result =>
            {

                var output = BsonSerializer.Deserialize<VectorSearchItem>(result);
                output.Distance = (float)result["score"].ToDouble();
                return output;
            })
            .ToArray(),
        };
    }
}

