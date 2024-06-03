using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using GenerativeAI.Tools;
using GenerativeAI.Types;

namespace LangChain.Providers.Google.Extensions;

internal static class GoogleGeminiExtensions
{
    public static bool IsFunctionCall(this EnhancedGenerateContentResponse response)
    {
        return response.GetFunction() != null;
    }

    public static List<GenerativeAITool> ToGenerativeAiTools(this IEnumerable<ChatCompletionFunction> functions)
    {
        return new List<GenerativeAITool>([
            new GenerativeAITool
            {
                FunctionDeclaration = [.. functions]
            }
        ]);
    }

    public static string GetString(this IDictionary<string, object>? arguments)
    {
        if (arguments == null)
            return string.Empty;

        var dictionary = arguments.ToDictionary(
            x => x.Key,
            x => x.Value.ToString() ?? string.Empty);
        
        return JsonSerializer.Serialize(dictionary, SourceGenerationContext.Default.DictionaryStringString);
    }
}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = [typeof(JsonStringEnumConverter)])]
[JsonSerializable(typeof(Dictionary<string, string>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;