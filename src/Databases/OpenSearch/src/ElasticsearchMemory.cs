// Copyright (c) Free Mind Labs, Inc. All rights reserved.

using System.Runtime.CompilerServices;
using LangChain.Providers;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.MemoryStorage;
using OpenSearch.Client;

namespace LangChain.Databases.OpenSearch;

/// <summary>
/// Elasticsearch connector for Kernel Memory.
/// </summary>
public class ElasticsearchMemory : IMemoryDb
{
    private readonly IEmbeddingModel _embeddingGenerator;
    private readonly IIndexNameHelper _indexNameHelper;
    private readonly OpenSearchVectorStoreOptions _config;
    private readonly IOpenSearchClient _client;

    /// <summary>
    /// Create a new instance of Elasticsearch KM connector
    /// </summary>
    /// <param name="options">Elasticsearch configuration</param>
    /// <param name="client">Elasticsearch client</param>
    /// <param name="log">Application logger</param>
    /// <param name="embeddingGenerator">Embedding generator</param>
    /// <param name="indexNameHelper">Index name helper</param>
    public ElasticsearchMemory(
        OpenSearchVectorStoreOptions options,
        IOpenSearchClient client,
        IEmbeddingModel embeddingGenerator,
        IIndexNameHelper indexNameHelper)
    {
        this._embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));
        this._indexNameHelper = indexNameHelper ?? throw new ArgumentNullException(nameof(indexNameHelper));
        this._config = options ?? throw new ArgumentNullException(nameof(options));
        this._client = client;// new ElasticsearchClient(this._config.ToElasticsearchClientSettings()); // TODO: inject
    }

    /// <inheritdoc />
    public async Task CreateIndexAsync(
        string index,
        int vectorSize,
        CancellationToken cancellationToken = default)
    {
        index = this._indexNameHelper.Convert(index);
        var existsResponse = await this._client.Indices.ExistsAsync(index, ct: cancellationToken).ConfigureAwait(false);
        if (existsResponse.Exists)
        {
            // this._log.LogTrace("{MethodName}: Index {Index} already exists.", nameof(CreateIndexAsync), index);
            return;
        }

        const int Dimensions = 1536; // TODO: make not hardcoded

        var np = new NestedProperty()
        {
            Properties = new Properties()
            {
                { ElasticsearchTag.NameField, new KeywordProperty() },
                { ElasticsearchTag.ValueField, new KeywordProperty() }
            }
        };

        var createIdxResponse = await this._client.Indices.CreateAsync(index,
            cfg =>
            {
                cfg.Settings(setts =>
                {
                    setts.NumberOfShards(this._config.ShardCount);
                    setts.NumberOfReplicas(1);
                    //setts.NumberOfReplicas(this._config.ConfigureProperties);
                    return setts;
                });

                cfg.Map(m => m.AutoMap<ElasticsearchMemoryRecord>()
                    //.Properties<ElasticsearchMemoryRecord>(p =>p
                    //    .Keyword(k => k.Name(f => f.Id))
                    //    .Nested()

                    );
                return cfg;
            },
            cancellationToken).ConfigureAwait(false);



        //var mapResponse = await this._client.Indices.PutMappingAsync(index, x => x
        //    .Properties<ElasticsearchMemoryRecord>(propDesc =>
        //    {
        //        propDesc.Keyword(x => x.Id);
        //        propDesc.Nested(ElasticsearchMemoryRecord.TagsField, np);
        //        propDesc.Text(x => x.Payload, pd => pd.Index(false));
        //        propDesc.Text(x => x.Content);
        //        propDesc.DenseVector(x => x.Vector, d => d.Index(true).Dims(Dimensions).Similarity("cosine"));

        //        this._config.ConfigureProperties?.Invoke(propDesc);
        //    }));

        //var mapResponse = await this._client.Indices.PutMappingAsync(index, x => x
        //    .Properties<ElasticsearchMemoryRecord>(propDesc =>
        //    {
        //        propDesc.Keyword(x => x.Id);
        //        propDesc.Nested(ElasticsearchMemoryRecord.TagsField, np);
        //        propDesc.Text(x => x.Payload, pd => pd.Index(false));
        //        propDesc.Text(x => x.Content);
        //        propDesc.DenseVector(x => x.Vector, d => d.Index(true).Dims(Dimensions).Similarity("cosine"));

        //        this._config.ConfigureProperties?.Invoke(propDesc);
        //    }),
        //    cancellationToken).ConfigureAwait(false);

        //this._log.LogTrace("{MethodName}: Index {Index} creeated.", nameof(CreateIndexAsync), index);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetIndexesAsync(
        CancellationToken cancellationToken = default)
    {
        var resp = await this._client.Indices.GetAsync(this._config.IndexPrefix + "*", ct: cancellationToken).ConfigureAwait(false);

        var names = resp.Indices
            .Select(x => x.Key.ToString().Replace(this._config.IndexPrefix, string.Empty))
            .ToList();
        //.ToHashSet(StringComparer.OrdinalIgnoreCase);

        // this._log.LogTrace("{MethodName}: Returned {IndexCount} indices: {Indices}.", nameof(GetIndexesAsync), names.Count, string.Join(", ", names));

        return names;
    }

    /// <inheritdoc />
    public async Task DeleteIndexAsync(
        string index,
        CancellationToken cancellationToken = default)
    {
        index = this._indexNameHelper.Convert(index);

        var delResponse = await this._client.Indices.DeleteAsync(index, ct: cancellationToken).ConfigureAwait(false);

        if (delResponse.Acknowledged)
        {
            //   this._log.LogTrace("{MethodName}: Index {Index} deleted.", nameof(DeleteIndexAsync), index);
        }
        else
        {
            //   this._log.LogWarning("{MethodName}: Index {Index} delete failed.", nameof(DeleteIndexAsync), index);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        string index,
        MemoryRecord record,
        CancellationToken cancellationToken = default)
    {
        index = this._indexNameHelper.Convert(index);

        record = record ?? throw new ArgumentNullException(nameof(record));

        var delResponse = await this._client.DeleteAsync<ElasticsearchMemoryRecord>(record.Id, (delReq) =>
            {
                delReq.Refresh(global::OpenSearch.Net.Refresh.WaitFor);
                return delReq;
            }, ct: cancellationToken)
            .ConfigureAwait(false);

        if (delResponse.IsValid)
        {
            //   this._log.LogTrace("{MethodName}: Record {RecordId} deleted.", nameof(DeleteAsync), record.Id);
        }
        else
        {
            //     this._log.LogWarning("{MethodName}: Record {RecordId} delete failed.", nameof(DeleteAsync), record.Id);
        }
    }

    /// <inheritdoc />
    public async Task<string> UpsertAsync(
        string index,
        MemoryRecord record,
        CancellationToken cancellationToken = default)
    {
        //index = this._indexNameHelper.Convert(index);

        ElasticsearchMemoryRecord memRec = ElasticsearchMemoryRecord.FromMemoryRecord(record);

        var response = await this._client.UpdateAsync<ElasticsearchMemoryRecord, ElasticsearchMemoryRecord>(
            memRec.Id,
            (updateReq) =>
            {
                updateReq.Index(index);
                updateReq.Refresh(global::OpenSearch.Net.Refresh.WaitFor);
                var memRec2 = memRec;
                updateReq.Doc(memRec2);
                updateReq.DocAsUpsert(true);
                return updateReq;
            }, ct: cancellationToken)
            .ConfigureAwait(false);

        if (response.IsValid)
        {
            //   this._log.LogTrace("{MethodName}: Record {RecordId} upserted.", nameof(UpsertAsync), memRec.Id);
        }
        else
        {
            //    this._log.LogError("{MethodName}: Record {RecordId} upsert failed.", nameof(UpsertAsync), memRec.Id);
        }

        return response.Id;
    }

    private Func<UpdateDescriptor<ElasticsearchMemoryRecord, ElasticsearchMemoryRecord>, IUpdateRequest<ElasticsearchMemoryRecord, ElasticsearchMemoryRecord>> UpdateRequest(string id) => descriptor => new UpdateRequest<ElasticsearchMemoryRecord, ElasticsearchMemoryRecord>(id);


    /// <inheritdoc />
    public async IAsyncEnumerable<(MemoryRecord, double)> GetSimilarListAsync(
        string index,
        string text,
        ICollection<MemoryFilter>? filters = null,
        double minRelevance = 0, int limit = 1, bool withEmbeddings = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (limit < 0)
        {
            limit = 10;
        }

        index = this._indexNameHelper.Convert(index);

        //this._log.LogTrace("{MethodName}: Searching for '{Text}' on index '{IndexName}' with filters {Filters}. {MinRelevance} {Limit} {WithEmbeddings}",
        //                   nameof(GetSimilarListAsync), text, index, filters.ToDebugString(), minRelevance, limit, withEmbeddings);

        EmbeddingResponse embeddingResponse = await this._embeddingGenerator.CreateEmbeddingsAsync(text, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var resp = await this._client.SearchAsync<ElasticsearchMemoryRecord>(s =>
            s.Index(index)
                .Query(k => k.
                    Knn(qd => 
                        qd.K(limit)
                            .Filter(q=>new QueryContainer())
                            .Field(x=>x.Vector)
                            .Vector(embeddingResponse)
                        )
             ),
             cancellationToken)
            .ConfigureAwait(false);

        if ((resp.HitsMetadata is null) || (resp.HitsMetadata.Hits is null))
        {
            //this._log.LogWarning("The search returned a null result. Should retry?");
            yield break;
        }

        foreach (var hit in resp.HitsMetadata.Hits)
        {
            if (hit?.Source == null)
            {
                continue;
            }

            //  this._log.LogTrace("{MethodName} Hit: {HitScore}, {HitId}", nameof(GetSimilarListAsync), hit.Score, hit.Id);
            yield return (hit.Source!.ToMemoryRecord(), hit.Score ?? 0);
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<MemoryRecord> GetListAsync(
        string index,
        ICollection<MemoryFilter>? filters = null,
        int limit = 1,
        bool withEmbeddings = false,
        [EnumeratorCancellation]
        CancellationToken cancellationToken = default)
    {
        //this._log.LogTrace("{MethodName}: querying index '{IndexName}' with filters {Filters}. {Limit} {WithEmbeddings}",
        //nameof(GetListAsync), index, filters.ToDebugString(), limit, withEmbeddings);

        if (limit < 0)
        {
            limit = 10;
        }

        index = this._indexNameHelper.Convert(index);

        var resp = await this._client.SearchAsync<ElasticsearchMemoryRecord>(s =>
            s.Index(index)
             .Size(limit)
             .Query(new Func<QueryContainerDescriptor<ElasticsearchMemoryRecord>, QueryContainer>(qd =>
             {
                 return qd.MatchAll();
             })),
             cancellationToken)
            .ConfigureAwait(false);

        if ((resp.HitsMetadata is null) || (resp.HitsMetadata.Hits is null))
        {
            yield break;
        }

        foreach (var hit in resp.Hits)
        {
            if (hit?.Source == null)
            {
                continue;
            }

            //  this._log.LogTrace("{MethodName} Hit: {HitScore}, {HitId}", nameof(GetListAsync), hit.Score, hit.Id);
            yield return hit.Source!.ToMemoryRecord();
        }
    }

    //private string ConvertIndexName(string index) => ESIndexName.Convert(this._config.IndexPrefix + index);

    //private QueryDescriptor<ElasticsearchMemoryRecord> ConvertTagFilters(
    //    QueryDescriptor<ElasticsearchMemoryRecord> qd,
    //    ICollection<MemoryFilter>? filters = null)
    //{
    //    if ((filters == null) || (filters.Count == 0))
    //    {
    //        qd.MatchAll();
    //        return qd;
    //    }

    //    filters = filters.Where(f => f.Keys.Count > 0)
    //                     .ToList(); // Remove empty filters

    //    if (filters.Count == 0)
    //    {
    //        qd.MatchAll();
    //        return qd;
    //    }

    //    foreach (MemoryFilter filter in filters)
    //    {
    //        List<Query> all = new();

    //        // Each tag collection is an element of a List<string, List<string?>>>
    //        foreach (var tagName in filter.Keys)
    //        {
    //            List<string?> tagValues = filter[tagName];
    //            List<FieldValue> terms = tagValues.Select(x => (FieldValue)(x ?? FieldValue.Null))
    //                                              .ToList();
    //            // ----------------
    //            Query newTagQuery = new TermQuery(ElasticsearchMemoryRecord.Tags_Name) { Value = tagName };
    //            newTagQuery &= new TermsQuery()
    //            {
    //                Field = ElasticsearchMemoryRecord.Tags_Value,
    //                Terms = new TermsQueryField(terms)
    //            };
    //            var nestedQd = new NestedQuery();
    //            nestedQd.Path = ElasticsearchMemoryRecord.TagsField;
    //            nestedQd.Query = newTagQuery;

    //            all.Add(nestedQd);
    //            qd.Bool(bq => bq.Must(all.ToArray()));
    //        }
    //    }

    // ---------------------

    //qd.Nested(nqd =>
    //{
    //    nqd.Path(ElasticsearchMemoryRecord.TagsField);

    //    nqd.Query(nq =>
    //    {
    //        // Each filter is a tag collection.
    //        foreach (MemoryFilter filter in filters)
    //        {
    //            List<Query> all = new();

    //            // Each tag collection is an element of a List<string, List<string?>>>
    //            foreach (var tagName in filter.Keys)
    //            {
    //                List<string?> tagValues = filter[tagName];
    //                List<FieldValue> terms = tagValues.Select(x => (FieldValue)(x ?? FieldValue.Null))
    //                                                  .ToList();
    //                // ----------------                        

    //                Query newTagQuery = new TermQuery(ElasticsearchMemoryRecord.Tags_Name) { Value = tagName };
    //                newTagQuery &= new TermsQuery() {
    //                    Field = ElasticsearchMemoryRecord.Tags_Value,
    //                    Terms = new TermsQueryField(terms)
    //                };

    //                all.Add(newTagQuery);
    //            }

    //            nq.Bool(bq => bq.Must(all.ToArray()));
    //        }
    //    });
    //});

    //     return qd;
    //   }
}

//public sealed partial class TermsQueryField : Union<IReadOnlyCollection<Elastic.Clients.Elasticsearch.FieldValue>, Elastic.Clients.Elasticsearch.QueryDsl.TermsLookup>
//{
//    public TermsQueryField(IReadOnlyCollection<Elastic.Clients.Elasticsearch.FieldValue> value) : base(value)
//    {
//    }

//    public TermsQueryField(Elastic.Clients.Elasticsearch.QueryDsl.TermsLookup lookup) : base(lookup)
//    {
//    }
//}