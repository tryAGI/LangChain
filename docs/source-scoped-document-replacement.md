# Replace chunks for one RAG data source

When an ingested PDF or text file changes, replacing its chunks must not delete chunks from other files. Semantic similarity search is **not** an accurate way to enumerate a file's chunks, and `MetadataJson` is not a portable filterable column across vector-store providers.

## Provider-independent option: retain the returned IDs

The `AddDocumentsFromAsync<TLoader>` collection extension already returns the IDs assigned to every chunk. Persist the mapping `source path -> chunk IDs` in your own durable store, and pass those IDs to `ReplaceSourceDocumentsFromAsync<TLoader>` when the file changes.

```csharp
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Schema;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

// Assume vectorCollection is an initialized
// VectorStoreCollection<string, LangChainDocumentRecord>
// and embeddingGenerator is an IEmbeddingGenerator<string, Embedding<float>>.
var path = "/documents/manual.txt";

// First ingestion: save these IDs in your application's durable source manifest.
var oldIds = await vectorCollection.AddDocumentsFromAsync<FileLoader>(
    embeddingGenerator,
    DataSource.FromPath(path),
    loaderSettings: new DocumentLoaderSettings());

// On subsequent updates:
var newIds = await vectorCollection.ReplaceSourceDocumentsFromAsync<FileLoader>(
    embeddingGenerator,
    source: path,
    previousDocumentIds: oldIds,
    dataSource: DataSource.FromPath(path));

// Update the persisted mapping only after success.
```

**Why this is safe for other sources:** Before any changes, the method reads *each* provided old record and checks its metadata: `path` must match the requested source (or `source` for custom documents without `path`). It also validates every replacement document's metadata. Incorrect or stale IDs, missing metadata, and source mismatches fail without starting the replacement. The comparison is exact and case-sensitive.

The method then generates and inserts new chunks **before** deleting the previously known IDs. Source metadata is collected automatically by the `FromAsync` replacement overload unless explicitly disabled in loader settings; disabling it causes validation to fail safely. The direct `ReplaceSourceDocumentsAsync` overload accepts already-loaded `Document` instances, including custom `source` metadata. Both typed and dynamically mapped vector collections are supported.

## Important limitations

- **Persist IDs:** These APIs do not implement a provider-wide metadata search. A durable external source-to-ID mapping is required, especially across process restarts. `VectorStore.AddDocumentsFromAsync<TLoader>` returns a collection, whereas the **collection** `AddDocumentsFromAsync<TLoader>` returns the chunk IDs.
- **Not transactional:** Vector-store providers do not share a cross-provider transaction. Write/delete failures or cancellation after insertion can leave both old and new chunks, or a partially replaced source. Reconcile the IDs from your durable manifest and the store; choose provider-native transactions if atomic replacement is essential.
- **No breaking schema migration:** No new fields are added to existing LangChain records, and no existing collection is recreated.
- **Empty source protection:** Empty replacement batches are rejected to avoid erasing a source due to a loader mistake. For intended removal, explicitly delete previously tracked IDs after your application's own validation.
- **Concurrency:** Serialize updates for a given source at the application layer and persist new IDs carefully; this helper does not provide optimistic concurrency or a distributed lock.

Related discussion: [Issue #446](https://github.com/tryAGI/LangChain/issues/446).
