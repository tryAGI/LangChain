namespace LangChain.Providers.Anthropic.Extensions;

public static class ToolExtensions
{
    //public static string ToToolXml(this IEnumerable<Tool> tools)
    //{
    //    List<string> toolXml = new List<string>();
    //    XmlDocument doc = new XmlDocument();
    //    var rootElement = doc.DocumentElement;
    //    var toolsElement = doc.CreateElement("tools");


    //    foreach (var tool in tools)
    //    {
    //        var function = tool.Function;

    //        XmlElement toolXmlElement = doc.CreateElement("tool_description");
    //        //Create Tool Name Element
    //        var toolName = doc.CreateElement("tool_name");
    //        toolName.AppendChild(doc.CreateTextNode(function.Name));

    //        //Create description Element
    //        var toolDescription = doc.CreateElement("description");
    //        toolDescription.AppendChild(doc.CreateTextNode(function.Description));

    //        //Append Tool Name element
    //        toolXmlElement.AppendChild(toolName);
    //        toolXmlElement.AppendChild(toolDescription);
    //        toolsElement.AppendChild(toolXmlElement);
    //    }

    //    rootElement.AppendChild(toolsElement);

    //    return doc.ToString();
    //}
}