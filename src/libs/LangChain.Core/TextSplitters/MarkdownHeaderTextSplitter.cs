using LangChain.Base;

namespace LangChain.TextSplitters;

public class MarkdownHeaderTextSplitter : TextSplitter
{
    private readonly bool _includeHeaders;

    private class LineType
    {
        public string Content { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
    }


    private string[] _headersToSplitOn;
    private const string _codeBlockseparator = "```";
    private readonly static string[] _defauldHeaders = {"#", "##", "###", "####", "#####", "######"};
    private readonly static string[] separator = {"\n"};

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
            .Split(separator, StringSplitOptions.None);


        var content = new List<LineType>();
        string currentHeader = null;
        int currentHeaderLen = 999; // determines current header level

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
                    Header = currentHeader ?? string.Empty,
                });
                continue;
            }

            bool headerFound = false;

            if (IsHeader(strippedLine, out int hLen) && hLen < currentHeaderLen)
            {
                currentHeader = strippedLine.TrimStart('#').Trim();
                currentHeaderLen = hLen;
                continue;
            }

            content.Add(new LineType()
            {
                Content = strippedLine.TrimStart('#'), // to avoid marking subheaders
                Header = currentHeader ?? string.Empty,
            });
        }

        List<string> result = new();
        foreach (var line in content.GroupBy(x=>x.Header))
        {
            var header = line.Key;
            var contentLines = string.Join("\n",line.Select(x => x.Content).ToList());
            if (_includeHeaders&&!string.IsNullOrEmpty(header))
            {
                result.Add(header + "\n" + contentLines);
                continue;
            }
            result.Add(contentLines);
        }

        return result;
    }

    
    private bool IsHeader(string line, out int len)
    {
        len = 0;
        foreach (var header in _headersToSplitOn)
        {
            if (line.StartsWith(header, StringComparison.Ordinal) && line[header.Length] == ' ')
            {
                len = header.Length;
                return true;
            }
        }

        return false;
    }
}