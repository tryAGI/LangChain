using LangChain.Base;
using LangChain.Docstore;
using LangChain.Indexes;
using LangChain.Providers;
using LangChain.Splitters.Text;
using LangChain.VectorStores;
using OpenSearch.Client;

namespace LangChain.Databases.OpenSearch
{
    public class OpenSearchVectorStore : VectorStore
    {
        private OpenSearchClient? _client;

        public static OpenSearchVectorStoreOptions DefaultOptions { get; } = new();

        public static async Task<VectorStoreIndexWrapper> GetOrCreateIndex(
                IEmbeddingModel embeddings,
                ISource source = null,
                OpenSearchVectorStoreOptions options = null
                )
        {
            options ??= DefaultOptions;

            TextSplitter textSplitter = new RecursiveCharacterTextSplitter(chunkSize: options.ChunkSize, chunkOverlap: options.ChunkOverlap);

            return null;
        }

        public OpenSearchVectorStore(string tableName, IEmbeddingModel embeddings)
            : base(embeddings)
        {
            _client = new OpenSearchClient();

        }

        public override Task<IEnumerable<string>> AddDocumentsAsync(IEnumerable<Document> documents, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<string>> AddTextsAsync(IEnumerable<string> texts, IEnumerable<Dictionary<string, object>>? metadatas = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<Document>> SimilaritySearchByVectorAsync(IEnumerable<float> embedding, int k = 4, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<(Document, float)>> SimilaritySearchWithScoreAsync(string query, int k = 4, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearchByVector(IEnumerable<float> embedding, int k = 4, int fetchK = 20, float lambdaMult = default,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearch(string query, int k = 4, int fetchK = 20, float lambdaMult = default,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Func<float, float> SelectRelevanceScoreFn()
        {
            throw new NotImplementedException();
        }
    }
}
