using System.Net;
using HtmlAgilityPack;

namespace LangChain.Providers.OpenRouter.CodeGenerator.Helpers;

public class DocumentHelper
{
    private string _DocHTML;

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
        get => _DocHTML;

        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                Document = new HtmlDocument();
                Document.LoadHtml(value);
                _DocHTML = value;
            }
            else
            {
                _DocHTML = "";
                Document = new HtmlDocument();
            }
        }
    }

    public HtmlNode FindNode(string TagName, string AttibuteName, string AttributeValue, bool Approx)
    {
        if (!Approx)
        {
            var htmlNode =
                Document.DocumentNode.SelectSingleNode(string.Format("//{0}[@{1}='{2}']", TagName.ToLower(),
                    AttibuteName, AttributeValue));

            if (htmlNode != null) return htmlNode;
        }
        else
        {
            foreach (var nd in Document.DocumentNode.Descendants(TagName.ToLower()))
                if (nd.Attributes[AttibuteName] != null)
                    if (!string.IsNullOrEmpty(nd.Attributes[AttibuteName].Value))
                        if (nd.Attributes[AttibuteName].Value.ToLower().Contains(AttributeValue.ToLower()))
                            return nd;
        }

        return null;
    }
}