using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LangChain.Databases.JsonConverters;

/// <inheritdoc />
/// According: https://stackoverflow.com/questions/65972825/c-sharp-deserializing-nested-json-to-nested-dictionarystring-object
[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
public sealed class ObjectAsPrimitiveConverter : JsonConverter<object>
{
    FloatFormat FloatFormat { get; init; }
    UnknownNumberFormat UnknownNumberFormat { get; init; }
    ObjectFormat ObjectFormat { get; init; }

    /// <inheritdoc />
    public ObjectAsPrimitiveConverter() : this(FloatFormat.Double, UnknownNumberFormat.Error, ObjectFormat.Expando) { }

    /// <inheritdoc />
    public ObjectAsPrimitiveConverter(FloatFormat floatFormat, UnknownNumberFormat unknownNumberFormat, ObjectFormat objectFormat)
    {
        FloatFormat = floatFormat;
        UnknownNumberFormat = unknownNumberFormat;
        ObjectFormat = objectFormat;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        value = value ?? throw new ArgumentNullException(nameof(value));
        writer = writer ?? throw new ArgumentNullException(nameof(writer));

        if (value.GetType() == typeof(object))
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
        else
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

    /// <inheritdoc />
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;

            case JsonTokenType.False:
                return false;

            case JsonTokenType.True:
                return true;

            case JsonTokenType.String:
                return reader.GetString();

            case JsonTokenType.Number:
                {
                    if (reader.TryGetInt32(out var i))
                        return i;

                    if (reader.TryGetInt64(out var l))
                        return l;

                    switch (FloatFormat)
                    {
                        // BigInteger could be added here.
                        case FloatFormat.Decimal when reader.TryGetDecimal(out var m):
                            return m;
                        case FloatFormat.Double when reader.TryGetDouble(out var d):
                            return d;
                    }

                    using var doc = JsonDocument.ParseValue(ref reader);
                    if (UnknownNumberFormat == UnknownNumberFormat.JsonElement)
                    {
                        return doc.RootElement.Clone();
                    }

                    throw new JsonException($"Cannot parse number {doc.RootElement}");
                }
            case JsonTokenType.StartArray:
                {
                    var list = new List<object?>();
                    while (reader.Read())
                    {
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.EndArray:
                                return list;

                            default:
                                list.Add(Read(ref reader, typeof(object), options));
                                break;
                        }
                    }
                    throw new JsonException();
                }
            case JsonTokenType.StartObject:
                var dict = CreateDictionary();
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.EndObject:
                            return dict;

                        case JsonTokenType.PropertyName:
                            var key = reader.GetString() ?? string.Empty;
                            reader.Read();
                            dict.Add(key, Read(ref reader, typeof(object), options));
                            break;

                        default:
                            throw new JsonException();
                    }
                }
                throw new JsonException();

            default:
                throw new JsonException($"Unknown token {reader.TokenType}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IDictionary<string, object?> CreateDictionary() =>
        ObjectFormat == ObjectFormat.Expando
            ? new ExpandoObject()
            : new Dictionary<string, object?>();
}

/// <summary> </summary>
public enum FloatFormat
{
    /// <summary> </summary>
    Double,
    /// <summary> </summary>
    Decimal
}

/// <summary>
/// 
/// </summary>
public enum UnknownNumberFormat
{
    /// <summary> </summary>
    Error,
    /// <summary> </summary>
    JsonElement,
}

/// <summary> </summary>
public enum ObjectFormat
{
    /// <summary> </summary>
    Expando,
    /// <summary> </summary>
    Dictionary,
}