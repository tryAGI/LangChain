using System.CodeDom;
using LangChain.Base;
using LangChain.Docstore;
using System.Linq;
using System.Xml.Linq;

namespace LangChain.TextSplitters;

public class MarkdownHeaderTextSplitter : TextSplitter
{
    private readonly bool _includeHeaders;

    public class LineType
    {
        public string Content { get; set; }
        public string Header { get; set; }
    }


    private string[] _headersToSplitOn;
    private static readonly string _codeBlockseparator = "```";
    private static readonly string[] _defauldHeaders = new[] {"#", "##", "###", "####", "#####", "######"};

    public MarkdownHeaderTextSplitter(string[]? headersToSplitOn = null, bool includeHeaders = true,
        bool groupByHeaders = false)
    {
        _includeHeaders = includeHeaders;

        _headersToSplitOn = headersToSplitOn ?? _defauldHeaders;
    }

    public override List<string> SplitText(string text)
    {
        // Split the input text by newline character ("\n").
        var lines = text
            .Replace("\r", "") // some people are using windows
            .Split(new[] {"\n"}, StringSplitOptions.None);


        var content = new List<LineType>();
        string currentHeader = null;
        int currentHeaderLen = 0; // determines current header level

        bool inCodeBlock = false;

        foreach (var line in lines)
        {
            var strippedLine = line.Trim();

            if (strippedLine.Length==0)
            {
                continue;
            }

            if (!inCodeBlock && strippedLine.StartsWith(_codeBlockseparator, StringComparison.InvariantCulture))
            {
                inCodeBlock = strippedLine.Split(new[] {_codeBlockseparator}, StringSplitOptions.None).Length < 2;
            }

            if (inCodeBlock)
            {
                content.Add(new LineType()
                {
                    Content = strippedLine,
                    Header = currentHeader
                });
                continue;
            }

            bool headerFound = false;

            if (IsHeader(strippedLine, out int hLen) && hLen > currentHeaderLen)
            {
                currentHeader = strippedLine.TrimStart('#').Trim();
                currentHeaderLen = hLen;
                continue;
            }

            content.Add(new LineType()
            {
                Content = strippedLine,
                Header = currentHeader
            });
        }

        return content.Select(line => (_includeHeaders ? line.Header + "\n" : "") + line.Content).ToList();
    }

    private bool IsHeader(string line, out int len)
    {
        len = 0;
        foreach (var header in _headersToSplitOn)
        {
            if (line.StartsWith(header) && line[header.Length] == ' ')
            {
                len = header.Length;
                return true;
            }
        }

        return false;
    }
}