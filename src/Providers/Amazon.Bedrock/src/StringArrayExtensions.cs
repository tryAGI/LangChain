using System.Text.Json.Nodes;

namespace LangChain.Providers.Amazon.Bedrock;

/// <summary>
/// 
/// </summary>
public static class StringArrayExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stringArray"></param>
    /// <returns></returns>
    public static JsonArray AsArray(this IReadOnlyList<string> stringArray)
    {
        stringArray = stringArray ?? throw new ArgumentNullException(nameof(stringArray));

        var jsonArray = new JsonArray();
        foreach (var arr in stringArray)
        {
            jsonArray.Add(arr);
        }
        return jsonArray;
    }
}