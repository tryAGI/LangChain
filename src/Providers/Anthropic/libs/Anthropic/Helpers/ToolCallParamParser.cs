using LangChain.Providers.Anthropic.Tools;

namespace LangChain.Providers.Anthropic.Helpers;

public class ToolCallParamParser
{
    public static object ParseData(string parameterName, AnthropicTool tool, string value)
    {
        var param = tool.Parameters?.FirstOrDefault(s => s.Name == parameterName);
        if (param != null)
            switch (param.Type)
            {
                case "integer":
                    if (param.Format == "int32")
                        return int.Parse(value);
                    return long.Parse(value);
                case "number":
                    if (param.Format == "float")
                        return float.Parse(value);
                    return double.Parse(value);
                case "string":
                    if (param.Format == "date-time")
                        return DateTime.Parse(value);
                    return value;
                case "boolean":
                    return bool.Parse(value);
                default:
                    return value;
            }

        return value;
    }
}