using LangChain.Sources;
using LangChain.Prompts.Base;
using LangChain.Schema;

namespace LangChain.Prompts;

/// <summary>
/// 
/// </summary>
public static class PromptHelpers
{
    /// <summary>
    /// Format a document into a string based on a prompt template.
    /// 
    /// First, this pulls information from the document from two sources: PageContent and Metadata
    /// Those are then passed into the `prompt` to produce a formatted string.
    /// </summary>
    /// <param name="doc">Document, the PageContent and Metadata will be used to create the final string.</param>
    /// <param name="prompt">BasePromptTemplate, will be used to format the PageContent and Metadata into the final string.</param>
    /// <param name="cancellationToken"></param>
    /// <returns> string of the document formatted.</returns>
    public static Task<string> FormatDocumentAsync(
        Document doc,
        BasePromptTemplate prompt,
        CancellationToken cancellationToken = default)
    {
        doc = doc ?? throw new ArgumentNullException(nameof(doc));
        prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));

        var data = new Dictionary<string, object>();

        var missing = new List<string>();
        foreach (var inputVar in prompt.InputVariables)
        {
            if (doc.Metadata.TryGetValue(inputVar, out var value))
            {
                data[inputVar] = value;
            }
            else if (inputVar == "page_content")
            {
                data[inputVar] = doc.PageContent;
            }
            else
            {
                missing.Add(inputVar);
            }
        }

        if (missing.Count != 0)
        {
            var requiredText = String.Join(",", prompt.InputVariables);
            var missingText = String.Join(",", missing);

            throw new ArgumentException(@$"
Document prompt requires documents to have metadata variables: {requiredText}.
Received document with missing metadata: {missingText}.");
        }

        return prompt.FormatAsync(new InputValues(data), cancellationToken);
    }
}