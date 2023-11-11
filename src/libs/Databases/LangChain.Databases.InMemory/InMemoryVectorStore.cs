using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LangChain.Abstractions.Embeddings.Base;
using LangChain.Docstore;
using LangChain.Indexes;
using LangChain.TextSplitters;
using LangChain.VectorStores;

namespace LangChain.Databases.InMemory
{
    public class InMemoryVectorStore : VectorStore
    {
        public static async Task<VectorStoreIndexWrapper> CreateIndexFromDocuments(IEmbeddings embeddings, List<Document> documents)
        {
            InMemoryVectorStore vectorStore = new InMemoryVectorStore(embeddings);
            var textSplitter = new CharacterTextSplitter();
            VectorStoreIndexCreator indexCreator = new VectorStoreIndexCreator(vectorStore, textSplitter);
            var index = await indexCreator.FromDocumentsAsync(documents).ConfigureAwait(false);
            return index;
        }

        private readonly Func<float[], float[], float> _distanceFunction;
        List<(float[] vec, string id, Document doc)> _storage = new List<(float[] vec, string id, Document doc)>();
        public InMemoryVectorStore(IEmbeddings embeddings, EDistanceMetrics distanceMetrics = EDistanceMetrics.Euclidean) : base(embeddings)
        {
            if (distanceMetrics == EDistanceMetrics.Euclidean)
                _distanceFunction = Utils.ComputeEuclideanDistance;
            else
                _distanceFunction = Utils.ComputeManhattanDistance;


        }



        public override async Task<IEnumerable<string>> AddDocumentsAsync(IEnumerable<Document> documents, CancellationToken cancellationToken = default)
        {

            var docs = documents.ToArray();

            var embeddings = await Embeddings.EmbedDocumentsAsync(docs.Select(x => x.PageContent).ToArray()).ConfigureAwait(false);
            List<string> ids = new List<string>();
            for (int i = 0; i < docs.Length; i++)
            {
                var id = Guid.NewGuid().ToString();
                ids.Add(id);
                _storage.Add((embeddings[i], id, docs[i]));
            }

            return ids;
        }

        public override async Task<IEnumerable<string>> AddTextsAsync(IEnumerable<string> texts, IEnumerable<Dictionary<string, object>>? metadatas = null, CancellationToken cancellationToken = default)
        {
            if (metadatas != null)
            {
                var docs = texts.Zip(metadatas, (d, m) => new Document(d, m)).ToArray();
                return await AddDocumentsAsync(docs, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var docs = texts.Select(d => new Document(d)).ToArray();
                return await AddDocumentsAsync(docs, cancellationToken).ConfigureAwait(false);
            }

        }

        public override Task<bool> DeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            _storage.RemoveAll(s => ids.Contains(s.id));
            return Task.FromResult(true);
        }

        public override async Task<IEnumerable<Document>> SimilaritySearchAsync(string query, int k = 4, CancellationToken cancellationToken = default)
        {
            var embedding = await Embeddings.EmbedQueryAsync(query).ConfigureAwait(false);
            return await SimilaritySearchByVectorAsync(embedding, k, cancellationToken).ConfigureAwait(false);
        }

        public override Task<IEnumerable<Document>> SimilaritySearchByVectorAsync(IEnumerable<float> embedding, int k = 4, CancellationToken cancellationToken = default)
        {

            var arr = embedding.ToArray();
            var distances = _storage.OrderBy(s => _distanceFunction(s.vec, arr)).Take(k);
            return Task.FromResult(distances.Select(d => d.doc));
        }

        public override async Task<IEnumerable<(Document, float)>> SimilaritySearchWithScoreAsync(string query,
            int k = 4, CancellationToken cancellationToken = default)
        {
            var embedding = await Embeddings.EmbedQueryAsync(query).ConfigureAwait(false);
            var arr = embedding.ToArray();
            var distances = _storage.Select(s =>
                new
                {
                    doc = s.doc,
                    distance = _distanceFunction(s.vec, arr)
                }).Take(k);
            return distances.Select(d => new ValueTuple<Document, float>(d.doc, d.distance));
        }

        public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearchByVector(IEnumerable<float> embedding, int k = 4, int fetchK = 20, float lambdaMult = 0.5f,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<Document>> MaxMarginalRelevanceSearch(string query, int k = 4, int fetchK = 20, float lambdaMult = 0.5f,
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