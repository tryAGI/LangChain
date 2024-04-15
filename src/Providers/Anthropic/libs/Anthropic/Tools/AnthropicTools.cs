using System.Xml.Serialization;
using LangChain.Providers.Anthropic.Helpers;

namespace LangChain.Providers.Anthropic.Tools;

[XmlRoot("tools_root")]
public class AnthropicTools
{
    [XmlArrayItem("tool_description")] public List<AnthropicTool> Tools { get; set; }

    public string ToXml()
    {
        var serializer =
            new XmlSerializer(typeof(AnthropicTools), null, null, new XmlRootAttribute("tools_root"), null);
        using var ms = new MemoryStream();
        serializer.Serialize(ms, this);
        ms.Seek(0, SeekOrigin.Begin);
        using var sr = new StreamReader(ms);
        var xml = sr.ReadToEnd();
        xml = xml.Replace("Parameters>", "parameters>");

        xml = TextHelper.GetTextBetweenDelimiters(xml, "<Tools>", "</Tools>");
        return $"<tools>{xml}</tools>";
    }
}