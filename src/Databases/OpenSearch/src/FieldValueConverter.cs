using System.Text.Json;
using System.Text.Json.Serialization;

namespace LangChain.Databases.OpenSearch;

internal sealed class FieldValueConverter : JsonConverter<FieldValue>
{
    public override FieldValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return FieldValue.Null;

            case JsonTokenType.String:
                var stringValue = reader.GetString();
                return FieldValue.String(stringValue);

            case JsonTokenType.Number:
                if (reader.TryGetInt64(out var l))
                {
                    return FieldValue.Long(l);
                }
                else if (reader.TryGetDouble(out var d))
                {
                    return FieldValue.Double(d);
                }
                else
                {
                    throw new JsonException("Unexpected number format which cannot be deserialised as a FieldValue.");
                }

            case JsonTokenType.True:
                return FieldValue.True;

            case JsonTokenType.False:
                return FieldValue.False;

            case JsonTokenType.StartObject:
            case JsonTokenType.StartArray:
                var value = JsonSerializer.Deserialize<LazyJson>(ref reader, options);
                return FieldValue.Any(value);
        }

        throw new JsonException($"Unexpected token type '{reader.TokenType}' read while deserializing a FieldValue.");
    }

    public override void Write(Utf8JsonWriter writer, FieldValue value, JsonSerializerOptions options)
    {
        if (value.TryGetString(out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }

        else if (value.TryGetBool(out var boolValue))
        {
            writer.WriteBooleanValue(boolValue.Value);
        }

        else if (value.TryGetLong(out var longValue))
        {
            writer.WriteNumberValue(longValue.Value);
        }

        else if (value.TryGetDouble(out var doubleValue))
        {
            writer.WriteNumberValue(doubleValue.Value);
        }

        else if (value.TryGetLazyDocument(out var lazyDocument))
        {
            writer.WriteRawValue(lazyDocument.Value.Bytes);
        }

        else if (value.TryGetComposite(out var objectValue))
        {
            if (!options.TryGetClientSettings(out var settings))
              throw // ThrowHelper.ThrowJsonExceptionForMissingSettings();

            SourceSerialization.Serialize(objectValue, objectValue.GetType(), writer, settings);
        }

        else if (value.Kind == FieldValue.ValueKind.Null)
        {
            writer.WriteNullValue();
        }

        else
        {
            throw new JsonException($"Unsupported FieldValue kind. This is likely a bug and should be reported as an issue.");
        }
    }
}