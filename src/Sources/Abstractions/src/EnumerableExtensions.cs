namespace LangChain.Sources;

/// <summary>
/// 
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// A read-only dictionary that maps file extensions to their corresponding MIME types.
    /// The dictionary uses case-insensitive string comparison.
    /// </summary>
    private static readonly Dictionary<string, string> Mappings = new(StringComparer.InvariantCultureIgnoreCase)
    {
        {".bmp", "image/bmp"},
        {".gif", "image/gif"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".png", "image/png"},
        {".tiff", "image/tiff"},
    };

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
            var contentType = GetMimeType(Path.GetExtension(path)) ?? "";
            var image = BinaryData.FromBytes(File.ReadAllBytes(path), contentType);

            images.Add(image);
        }

        return images;
    }

    /// <summary>
    /// Retrieves the MIME type associated with the specified file extension.
    /// If the file extension is not found in the mappings, the "application/octet-stream" MIME type is returned.
    /// </summary>
    /// <param name="extension">The file extension to look up, with or without the leading period. If null, an ArgumentNullException is thrown.</param>
    /// <returns>The MIME type associated with the specified file extension, or "application/octet-stream" if the extension is not found.</returns>
    public static string GetMimeType(string extension)
    {
        extension = extension ?? throw new ArgumentNullException(nameof(extension));

        if (extension.StartsWith(".", StringComparison.InvariantCultureIgnoreCase) == false)
        {
            extension = "." + extension;
        }

        return Mappings.TryGetValue(extension, out var mime) ? mime : "application/octet-stream";
    }
}