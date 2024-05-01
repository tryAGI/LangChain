using System.Net.Http;
using System.Text;

namespace LangChain.Sources;

/// <summary>
/// Stream will be disposed after use.
/// </summary>
public sealed class DataSource
{
    public required DataSourceType Type { get; init; }
    public string? Value { get; init; }
    public Stream? Stream { get; init; }
    public Encoding Encoding { get; init; } = Encoding.UTF8;
    
    public static DataSource FromPath(string path)
    {
        path = path ?? throw new ArgumentNullException(nameof(path));
        
        return new DataSource
        {
            Type = DataSourceType.Path,
            Value = path,
        };
    }
    
    public static DataSource FromUri(Uri uri)
    {
        uri = uri ?? throw new ArgumentNullException(nameof(uri));
        
        return new DataSource
        {
            Type = DataSourceType.Uri,
            Value = uri.ToString(),
        };
    }

    public static DataSource FromUrl(string url)
    {
        url = url ?? throw new ArgumentNullException(nameof(url));
        
        return new DataSource
        {
            Type = DataSourceType.Uri,
            Value = url,
        };
    }

    public static DataSource FromStream(Stream stream)
    {
        stream = stream ?? throw new ArgumentNullException(nameof(stream));
        
        return new DataSource
        {
            Type = DataSourceType.Stream,
            Stream = stream,
        };
    }
    
    public async Task<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        if (Stream is not null)
        {
            return Stream;
        }

        return Type switch
        {
            DataSourceType.Uri => await DownloadAsMemoryStreamAsync(
                    new Uri(Value!),
                    cancellationToken)
                .ConfigureAwait(false),
            DataSourceType.Stream => Stream!,
            DataSourceType.Path => new FileStream(Value!, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous),
            _ => new MemoryStream(Encoding.GetBytes(Value!)),
        };
    }
    
    private static async Task<MemoryStream> DownloadAsMemoryStreamAsync(
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        using var client = new HttpClient();
        using var stream = await client.GetStreamAsync(uri, cancellationToken).ConfigureAwait(false);

        await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        memoryStream.Position = 0;

        return memoryStream;
    }
}