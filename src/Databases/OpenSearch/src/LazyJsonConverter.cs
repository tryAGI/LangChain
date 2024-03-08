using System.Text.Json;
using System.Text.Json.Serialization;

namespace LangChain.Databases.OpenSearch;

internal sealed class LazyJsonConverter : JsonConverter<LazyJson>
{
    private IElasticsearchClientSettings _settings;

    public override LazyJson Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        InitializeSettings(options);

        using var jsonDoc = JsonSerializer.Deserialize<JsonDocument>(ref reader);
        using var stream = _settings.MemoryStreamFactory.Create();

        var writer = new Utf8JsonWriter(stream);
        jsonDoc.WriteTo(writer);
        writer.Flush();

        return new LazyJson(stream.ToArray(), _settings);
    }

    public override void Write(Utf8JsonWriter writer, LazyJson value, JsonSerializerOptions options) => throw new NotImplementedException("We only ever expect to deserialize LazyJson on responses.");

    private void InitializeSettings(JsonSerializerOptions options)
    {
        if (_settings is null)
        {
            if (!options.TryGetClientSettings(out var settings))
                ThrowHelper.ThrowJsonExceptionForMissingSettings();

            _settings = settings;
        }
    }
}