using System.Globalization;
using LangChain.Providers.Anthropic.Tools;

namespace LangChain.Providers.Anthropic.Helpers;

public static class ToolCallParamParser
{
    public static object ParseData(string parameterName, AnthropicTool? tool, string value)
    {
        var param = tool?.Parameters.FirstOrDefault(s => s.Name == parameterName);
        if (param != null)
            switch (param.Type)
            {
                case "integer":
                    if (param.Format == "int32")
                        return int.Parse(value, CultureInfo.InvariantCulture);
                    return long.Parse(value, CultureInfo.InvariantCulture);
                case "number":
                    if (param.Format == "float")
                        return float.Parse(value, CultureInfo.InvariantCulture);
                    return double.Parse(value, CultureInfo.InvariantCulture);
                case "string":
                    if (param.Format == "date-time")
                        return DateTime.Parse(value, CultureInfo.InvariantCulture);
                    return value;
                case "boolean":
                    return bool.Parse(value);
                default:
                    return value;
            }

        return value;
    }
}