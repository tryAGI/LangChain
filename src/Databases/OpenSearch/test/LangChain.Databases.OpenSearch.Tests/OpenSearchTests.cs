using Amazon;
using LangChain.Providers.Amazon.Bedrock;
using LangChain.Providers.Amazon.Bedrock.Predefined.Amazon;
using LangChain.Sources;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.MemoryStorage;
using Microsoft.SemanticKernel.Text;
using System.Globalization;
using LangChain.Providers;
using static LangChain.Chains.Chain;
using Microsoft.SemanticKernel.AI.Embeddings;
using OpenSearch.Client;

namespace LangChain.Databases.OpenSearch.Tests
{
    public class OpenSearchTests
    {
        public IMemoryDb MemoryDb { get; set; }
        public IEmbeddingModel TextEmbeddingGenerator { get; set; }
        public IOpenSearchClient OpenSearchClient { get; set; }

        [SetUp]
        public void Setup()
        {
            OpenSearchClient = new OpenSearchClient();

            var provider = new BedrockProvider(RegionEndpoint.USWest2);
            TextEmbeddingGenerator = new TitanEmbedTextV1Model(provider);
            var llm = new TitanTextExpressV1Model(provider);

            MemoryDb = new ElasticsearchMemory(
                new OpenSearchVectorStoreOptions(), 
                OpenSearchClient, 
                TextEmbeddingGenerator,
                new IndexNameHelper(new OpenSearchVectorStoreOptions()));
        }

        [Test]
        public async Task Test1()
        {
            var provider = new BedrockProvider(RegionEndpoint.USWest2);
            var embeddings = new TitanEmbedTextV1Model(provider);
            var llm = new TitanTextExpressV1Model(provider);

            var bookURL =
                "https://canonburyprimaryschool.co.uk/wp-content/uploads/2016/01/Joanne-K.-Rowling-Harry-Potter-Book-1-Harry-Potter-and-the-Philosophers-Stone-EnglishOnlineClub.com_.pdf";
            var source = await PdfPigPdfSource.CreateFromUriAsync(new Uri(bookURL));
            var index = await OpenSearchVectorStore.GetOrCreateIndex(embeddings, source);

            string promptText =
                @"Use the following pieces of context to answer the question at the end. If the answer is not in context then just say that you don't know, don't try to make up an answer. Keep the answer as short as possible. Always quote the context in your answer.

{context}

Question: {text}
Helpful Answer:";


            var chain =
                Set("Who was drinking a unicorn blood?")    // set the question
                | RetrieveDocuments(index, amount: 5)       // take 5 most similar documents
                | StuffDocuments(outputKey: "context")      // combine documents together and put them into context
                | Template(promptText)                      // replace context and question in the prompt with their values
                | LLM(llm);                                 // send the result to the language model

            var answer = await chain.Run("text");         // get chain result
            Console.WriteLine("Answer:" + answer);       // print the result
            // print usage
            Console.WriteLine($"LLM usage: {llm.Usage}");
            Console.WriteLine($"Embeddings usage: {embeddings.Usage}");
        }

        [Test]
        public async Task can_upsert_one_text_document()
        {
            var datafile = Path.Combine(Path.GetTempPath(), "data/file1-Wikipedia-Carbon.txt");

            var docIds = await UpsertTextFilesAsync(
                memoryDb: this.MemoryDb,
                textEmbeddingGenerator: this.TextEmbeddingGenerator,
                indexName: nameof(can_upsert_one_text_document),
                fileNames: new[]
                {
                    datafile
                }).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<string>> UpsertTextFilesAsync(
           IMemoryDb memoryDb,
           IEmbeddingModel textEmbeddingGenerator,
           string indexName,
           IEnumerable<string> fileNames)
        {
            ArgumentNullException.ThrowIfNull(memoryDb);
            ArgumentNullException.ThrowIfNull(textEmbeddingGenerator);
            ArgumentNullException.ThrowIfNull(indexName);
            ArgumentNullException.ThrowIfNull(fileNames);

            // IMemoryDb does not create the index automatically.
            await memoryDb.CreateIndexAsync(indexName, 1536)
                          .ConfigureAwait(false);

            var results = new List<string>();
            foreach (var fileName in fileNames)
            {
                // Reads the text from the file
                string fullText = await File.ReadAllTextAsync(fileName)
                                            .ConfigureAwait(false);

                // Splits the text into lines of up to 1000 tokens each
                var lines = TextChunker.SplitPlainTextLines(fullText,
                    maxTokensPerLine: 1000);

                // Splits the line into paragraphs
                var paragraphs = TextChunker.SplitPlainTextParagraphs(lines,
                    maxTokensPerParagraph: 1000,
                    overlapTokens: 100);

                Console.WriteLine($"File '{fileName}' contains {paragraphs.Count} paragraphs.");

                // Indexes each paragraph as a separate document
                var paraIdx = 0;
                var documentId = GuidWithoutDashes() + GuidWithoutDashes();
                var fileId = GuidWithoutDashes();

                foreach (var paragraph in paragraphs)
                {
                    var embedding = await textEmbeddingGenerator.CreateEmbeddingsAsync(paragraph)
                                                                     .ConfigureAwait(false);

                    Console.WriteLine($"Indexed paragraph {++paraIdx}/{paragraphs.Count}. {paragraph.Length} characters.");

                    var filePartId = GuidWithoutDashes();

                    var esId = $"d={documentId}//p={filePartId}";

                    var mrec = new MemoryRecord()
                    {
                        Id = esId,
                        Payload = new Dictionary<string, object>()
                    {
                        { "file", fileName },
                        { "text", paragraph },
                        { "vector_provider", textEmbeddingGenerator.GetType().Name },
                        { "vector_generator", "TODO" },
                        { "last_update", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") },
                        { "text_embedding_generator", textEmbeddingGenerator.GetType().Name }
                    },
                        Tags = new TagCollection()
                    {
                        { "__document_id", documentId },
                        { "__file_type", "text/plain" },
                        { "__file_id", fileId },
                        { "__file_part", filePartId }

                    },
                        Vector = embedding.ToSingleArray()
                    };

                    var res = await memoryDb.UpsertAsync(indexName, mrec)
                                                 .ConfigureAwait(false);

                    results.Add(res);
                }

                Console.WriteLine("");
            }

            return results;
        }

        public static string GuidWithoutDashes() => Guid.NewGuid().ToString().Replace("-", "", StringComparison.OrdinalIgnoreCase).ToLower(CultureInfo.CurrentCulture);

    }
}