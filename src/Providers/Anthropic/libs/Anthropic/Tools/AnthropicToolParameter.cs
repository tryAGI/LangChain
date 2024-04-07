using System.Xml.Serialization;

namespace LangChain.Providers.Anthropic.Tools;

public class AnthropicToolParameter
{
    [XmlElement(ElementName = "name")] public string? Name { get; set; }

    [XmlElement(ElementName = "type")] public string? Type { get; set; }

    [XmlElement(ElementName = "description")]
    public string? Description { get; set; }
    [XmlIgnore] public string? Format { get; set; }
}