using System.Collections;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LangChain.Schema;

internal sealed class LangChainJsonObjectConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        return document.RootElement.Clone();
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        WriteValue(writer, value);
    }

    private static void WriteValue(Utf8JsonWriter writer, object? value)
    {
        switch (value)
        {
            case null:
                writer.WriteNullValue();
                return;
            case JsonElement element:
                element.WriteTo(writer);
                return;
            case JsonDocument document:
                document.WriteTo(writer);
                return;
            case string text:
                writer.WriteStringValue(text);
                return;
            case char character:
                writer.WriteStringValue(character.ToString());
                return;
            case bool boolean:
                writer.WriteBooleanValue(boolean);
                return;
            case byte number:
                writer.WriteNumberValue(number);
                return;
            case sbyte number:
                writer.WriteNumberValue(number);
                return;
            case short number:
                writer.WriteNumberValue(number);
                return;
            case ushort number:
                writer.WriteNumberValue(number);
                return;
            case int number:
                writer.WriteNumberValue(number);
                return;
            case uint number:
                writer.WriteNumberValue(number);
                return;
            case long number:
                writer.WriteNumberValue(number);
                return;
            case ulong number:
                writer.WriteNumberValue(number);
                return;
            case decimal number:
                writer.WriteNumberValue(number);
                return;
            case float number:
                writer.WriteNumberValue(number);
                return;
            case double number:
                writer.WriteNumberValue(number);
                return;
            case DateTime dateTime:
                writer.WriteStringValue(dateTime);
                return;
            case DateTimeOffset dateTimeOffset:
                writer.WriteStringValue(dateTimeOffset);
                return;
            case Guid guid:
                writer.WriteStringValue(guid);
                return;
            case TimeSpan timeSpan:
                writer.WriteStringValue(timeSpan.ToString("c", CultureInfo.InvariantCulture));
                return;
            case byte[] bytes:
                writer.WriteBase64StringValue(bytes);
                return;
            case Enum enumValue:
                WriteEnum(writer, enumValue);
                return;
            case IDictionary dictionary:
                writer.WriteStartObject();
                foreach (DictionaryEntry entry in dictionary)
                {
                    if (entry.Key is not string key)
                    {
                        throw new NotSupportedException("JSON metadata dictionaries must use string keys.");
                    }

                    writer.WritePropertyName(key);
                    WriteValue(writer, entry.Value);
                }

                writer.WriteEndObject();
                return;
            case IEnumerable values:
                writer.WriteStartArray();
                foreach (var item in values)
                {
                    WriteValue(writer, item);
                }

                writer.WriteEndArray();
                return;
        }

#if NET8_0_OR_GREATER
        if (JsonSerializer.IsReflectionEnabledByDefault &&
            System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        {
            JsonSerializer.Serialize(writer, value, value.GetType());
            return;
        }
#else
        JsonSerializer.Serialize(writer, value, value.GetType());
        return;
#endif

        throw new NotSupportedException(
            $"Metadata values of type '{value.GetType()}' require reflection-based JSON serialization, which is disabled. " +
            "Use JSON-compatible metadata values.");
    }

    private static void WriteEnum(Utf8JsonWriter writer, Enum value)
    {
        var underlyingType = Enum.GetUnderlyingType(value.GetType());
        if (underlyingType == typeof(ulong))
        {
            writer.WriteNumberValue(Convert.ToUInt64(value, CultureInfo.InvariantCulture));
        }
        else
        {
            writer.WriteNumberValue(Convert.ToInt64(value, CultureInfo.InvariantCulture));
        }
    }
}
