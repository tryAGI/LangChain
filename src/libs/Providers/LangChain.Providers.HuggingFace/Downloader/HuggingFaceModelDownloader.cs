namespace LangChain.Providers.HuggingFace.Downloader;

/// <summary>
/// A downloader for HuggingFace models
/// </summary>
public class HuggingFaceModelDownloader
{
    /// <summary>
    /// 
    /// </summary>
    public static HuggingFaceModelDownloader Instance { get; } = new HuggingFaceModelDownloader();


    /// <summary>
    /// The HttpClient used to download the models
    /// </summary>
    public HttpClient HttpClient { get; set; } = new HttpClient();

    /// <summary>
    /// The default storage path for the models
    /// </summary>
    public static string DefaultStoragePath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LangChain", "CSharp", "Models");

    private async Task DownloadModel(string url, string path, CancellationToken? cancellationToken = null)
    {
        var client = HttpClient;

        using (var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            using ProgressBar progress = new ProgressBar();

            await client.DownloadAsync(url, file, progress, cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Downloads a model from HuggingFace with caching and return path to it
    /// </summary>
    public async Task<string> GetModel(string repository, string fileName, string version = "master", string? storagePath = null)
    {
        storagePath ??= HuggingFaceModelDownloader.DefaultStoragePath;
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
            var downloadUrl = $"https://huggingface.co/{repository}/resolve/{version}/{fileName}";
            await DownloadModel(downloadUrl, modelPath).ConfigureAwait(false);
            File.Delete(downloadMarkerPath);
        }


        return modelPath;
    }
}