using System.Text.Json;
using LangChain.DocumentLoaders;
using LangChain.Schema;
using Microsoft.Extensions.VectorData;

namespace LangChain.Extensions;

internal static class LangChainDocumentRecordMapper
{
    internal static VectorStoreCollectionDefinition CreateCollectionDefinition(int dimensions)
    {
        return new VectorStoreCollectionDefinition
        {
            Properties =
            [
                new VectorStoreKeyProperty("Id", typeof(string)),
                new VectorStoreDataProperty("Text", typeof(string)),
                new VectorStoreDataProperty("MetadataJson", typeof(string)),
                new VectorStoreVectorProperty("Embedding", dimensions) { Type = typeof(ReadOnlyMemory<float>) },
            ]
        };
    }

    internal static Dictionary<string, object?> ToDynamicRecord(LangChainDocumentRecord record)
    {
        return new Dictionary<string, object?>
        {
            ["Id"] = record.Id,
            ["Text"] = record.Text,
            ["MetadataJson"] = record.MetadataJson,
            ["Embedding"] = record.Embedding,
        };
    }

    internal static Document RecordToDocument(IDictionary<string, object?> record)
    {
        return RecordToDocument(
            GetString(record, "Text"),
            GetString(record, "MetadataJson"));
    }

    internal static Document RecordToDocument(LangChainDocumentRecord record)
    {
        return RecordToDocument(record.Text, record.MetadataJson);
    }

    private static Document RecordToDocument(string? text, string? metadataJson)
    {
        Dictionary<string, object>? metadata = null;
        if (metadataJson is { Length: > 0 })
        {
            metadata = JsonSerializer.Deserialize(
                metadataJson,
                LangChainJsonSerializerContext.Metadata.DictionaryStringObject);
        }

        return new Document(text ?? string.Empty, metadata);
    }

    private static string? GetString(IDictionary<string, object?> values, string name)
    {
        if (!values.TryGetValue(name, out var value) || value is null)
        {
            return null;
        }

        return value switch
        {
            string text => text,
            JsonElement { ValueKind: JsonValueKind.String } element => element.GetString(),
            JsonElement { ValueKind: JsonValueKind.Null } => null,
            _ => throw InvalidRecord(name),
        };
    }

    private static InvalidOperationException InvalidRecord(string propertyName)
    {
        return new InvalidOperationException(
            $"The dynamically mapped vector record has an invalid or missing '{propertyName}' property.");
    }
}
