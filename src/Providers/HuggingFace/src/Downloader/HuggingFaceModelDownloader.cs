using Uri = System.Uri;

namespace LangChain.Providers.HuggingFace.Downloader;

/// <summary>
/// A downloader for HuggingFace models
/// </summary>
public static class HuggingFaceModelDownloader
{
    /// <summary>
    /// The default storage path for the models
    /// </summary>
    public static string DefaultStoragePath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LangChain", "CSharp", "Models");

    private static async Task DownloadModelAsync(Uri uri, string path, CancellationToken? cancellationToken = null)
    {
        using var client = new HttpClient();
        using var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        using var progress = new ProgressBar();

        await client.DownloadAsync(uri, file, progress, cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a model from HuggingFace with caching and return path to it
    /// </summary>
    public static async Task<string> GetModelAsync(
        string repository,
        string fileName,
        string version = "master",
        string? storagePath = null,
        CancellationToken cancellationToken = default)
    {
        storagePath ??= DefaultStoragePath;
        var repositoryPath = Path.Combine(storagePath, repository);
        if (!Directory.Exists(repositoryPath))
        {
            Directory.CreateDirectory(repositoryPath);
        }

        var modelPath = Path.Combine(repositoryPath, version, fileName);
        var directory = Path.GetDirectoryName(modelPath) ?? string.Empty;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        var downloadMarkerPath = modelPath + ".hfdownload"; // to verify if the download is complete
        if (!File.Exists(modelPath) || File.Exists(downloadMarkerPath))
        {
            File.WriteAllText(downloadMarkerPath, "");
            File.Delete(modelPath);
            Console.WriteLine("No model file found. Downloading...");

            await DownloadModelAsync(new Uri($"https://huggingface.co/{repository}/resolve/{version}/{fileName}"), modelPath).ConfigureAwait(false);

            File.Delete(downloadMarkerPath);
        }

        return modelPath;
    }
}