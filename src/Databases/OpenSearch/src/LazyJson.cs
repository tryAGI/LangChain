using System.Text.Json.Serialization;

namespace LangChain.Databases.OpenSearch;

/// <summary>
/// <para>Lazily deserializable JSON.</para>
/// <para>Holds raw JSON bytes which can be lazily deserialized to a specific <see cref="Type"/> using the source serializer at a later time.</para>
/// </summary>
[JsonConverter(typeof(LazyJsonConverter))]
public readonly struct LazyJson
{
    internal LazyJson(byte[] bytes, IElasticsearchClientSettings settings)
    {
        Bytes = bytes;
        Settings = settings;
    }

    internal byte[]? Bytes { get; }
    internal IElasticsearchClientSettings? Settings { get; }

    /// <summary>
    /// Creates an instance of <typeparamref name="T" /> from this
    /// <see cref="LazyJson" /> instance.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    public T? As<T>()
    {
        if (Bytes is null || Settings is null || Bytes.Length == 0)
            return default;

        using var ms = Settings.MemoryStreamFactory.Create(Bytes);
        return Settings.SourceSerializer.Deserialize<T>(ms);
    }
}