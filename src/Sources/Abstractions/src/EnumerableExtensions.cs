using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Generic;

namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="texts"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IReadOnlyList<Document> ToDocuments(this IEnumerable<string> texts)
    {
        texts = texts ?? throw new ArgumentNullException(nameof(texts));

        return texts
            .Select(text => new Document(text))
            .ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IReadOnlyList<BinaryData> ToBinaryData(this IEnumerable<string> paths)
    {
        paths = paths ?? throw new ArgumentNullException(nameof(paths));
       var images = new List<BinaryData>();

        foreach (var path in paths)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(path, out var contentType);

            var image = BinaryData.FromBytes(File.ReadAllBytes(path), contentType);

            images.Add(image);
        }

        return images;
    }
}