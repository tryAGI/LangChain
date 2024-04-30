using System.Xml.Serialization;

namespace LangChain.Providers.Anthropic.Tools;

[XmlRoot(ElementName = "tool_description")]
public class AnthropicTool
{
    [XmlElement(ElementName = "description")]
    public string Description { get; set; } = string.Empty;

    [XmlElement(ElementName = "tool_name")]
    public string Name { get; set; } = string.Empty;

    [XmlArrayItem(ElementName = "parameter")]
    public List<AnthropicToolParameter> Parameters { get; set; } = [];
}