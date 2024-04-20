using System.Globalization;
using System.Net;
using HtmlAgilityPack;

namespace LangChain.Providers.OpenRouter.CodeGenerator.Helpers;

public class DocumentHelper
{
    private string _docHtml = string.Empty;

    public DocumentHelper()
    {
        Document = new HtmlDocument();
        DocumentText = "";
        ServicePointManager.Expect100Continue = false;

        HtmlNode.ElementsFlags.Remove("form");
        HtmlNode.ElementsFlags.Remove("option");
    }

    public HtmlDocument Document { get; set; }

    public string DocumentText
    {
        get => _docHtml;

        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                Document = new HtmlDocument();
                Document.LoadHtml(value);
                _docHtml = value;
            }
            else
            {
                _docHtml = "";
                Document = new HtmlDocument();
            }
        }
    }

    public HtmlNode? FindNode(string tagName, string attributeName, string attributeValue, bool approx)
    {
        tagName = tagName ?? throw new ArgumentNullException(nameof(tagName));
        
        if (!approx)
        {
            var htmlNode =
                Document.DocumentNode.SelectSingleNode(string.Format("//{0}[@{1}='{2}']", tagName.ToLower(CultureInfo.InvariantCulture),
                    attributeName, attributeValue));

            if (htmlNode != null) return htmlNode;
        }
        else
        {
            foreach (var nd in Document.DocumentNode.Descendants(tagName.ToLower(CultureInfo.InvariantCulture)))
                if (nd.Attributes[attributeName] != null)
                    if (!string.IsNullOrEmpty(nd.Attributes[attributeName].Value))
                        if (nd.Attributes[attributeName].Value.Contains(attributeValue, StringComparison.InvariantCultureIgnoreCase))
                            return nd;
        }

        return null;
    }
}