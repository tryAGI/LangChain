namespace LangChain.Splitters.Text;

/// <inheritdoc/>
public class MarkdownHeaderTextSplitter : TextSplitter
{
    private readonly bool _includeHeaders;

    private sealed class LineType
    {
        public string Content { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
    }


    private string[] _headersToSplitOn;
    private const string _codeBlockseparator = "```";
    private readonly static string[] _defauldHeaders = { "#", "##", "###", "####", "#####", "######" };
    private readonly static string[] separator = { "\n" };

    /// <inheritdoc/>
    public MarkdownHeaderTextSplitter(
        string[]? headersToSplitOn = null,
        bool includeHeaders = true)
    {
        _includeHeaders = includeHeaders;

        _headersToSplitOn = headersToSplitOn ?? _defauldHeaders;
    }



    /// <inheritdoc/>
    public override IReadOnlyList<string> SplitText(string text)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));

        // Split the input text by newline character ("\n").
        var lines = text
            .Replace("\r", "") // some people are using windows
            .Split(separator, StringSplitOptions.None);


        var content = new List<LineType>();
        string currentHeader = string.Empty;
        int currentHeaderLen = 999; // determines current header level

        bool inCodeBlock = false;

        foreach (var line in lines)
        {
            var strippedLine = line.Trim();

            if (strippedLine.Length == 0)
            {
                continue;
            }

            if (!inCodeBlock && strippedLine.StartsWith(_codeBlockseparator, StringComparison.InvariantCulture))
            {
                inCodeBlock = strippedLine.Split(new[] { _codeBlockseparator }, StringSplitOptions.None).Length < 2;
            }

            if (inCodeBlock)
            {
                content.Add(new LineType()
                {
                    Content = strippedLine,
                    Header = currentHeader,
                });
                continue;
            }

            if (IsHeader(strippedLine, out int hLen))
            {
                if (hLen <= currentHeaderLen)
                {
                    var existingHeader = currentHeader.Split('|');

                    string prevHeader = string.Join("|", existingHeader.Take(existingHeader.Length - 1));
                    currentHeader = prevHeader + "|" + strippedLine.TrimStart('#').Trim();
                    currentHeaderLen = hLen;
                    continue;
                }

                if (hLen > currentHeaderLen)
                {
                    currentHeader += "|";
                    currentHeader += strippedLine.TrimStart('#').Trim();
                    currentHeaderLen = hLen;
                    continue;
                }
            }

            content.Add(new LineType()
            {
                Content = strippedLine.TrimStart('#'), // to avoid marking subheaders
                Header = currentHeader,
            });
        }

        List<string> result = new();
        foreach (var line in content.GroupBy(x => x.Header))
        {
            var header = line.Key;

            var contentLines = string.Join("\n", line.Select(x => x.Content).ToList());
            if (_includeHeaders && !string.IsNullOrEmpty(header))
            {
                header = string.Join(": ", header.Trim('|').Split('|'));
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
            if (line.Length <= header.Length + 1)
                return false;//Empty lines starting with #s should not be considered as headers. Removing this line would result in exceptions in that conditions
            if (line.Trim().StartsWith(header, StringComparison.Ordinal) && line[header.Length] == ' ')
            {
                len = header.Length;
                return true;
            }
        }

        return false;
    }
}
