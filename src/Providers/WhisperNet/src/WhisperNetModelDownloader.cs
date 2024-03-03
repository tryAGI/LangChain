using Whisper.net.Ggml;

namespace LangChain.Providers.WhisperNet;

public sealed class WhisperNetModelDownloader
{
    /// <summary>
    /// The default storage path for the models
    /// </summary>
    /// <returns>Model path</returns>
    public static string DefaultStoragePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LangChain", "CSharp", "Models", "WhisperNet");
    
    public static async Task<string> DownloadModel(GgmlType type, QuantizationType quantizationType, string? storagePath = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        storagePath ??= DefaultStoragePath;
        using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(type, quantizationType, cancellationToken).ConfigureAwait(false);
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
        }
#pragma warning disable CA1308
        var fileName = $"whisper_gglm_{type.ToString().ToLowerInvariant()}";
        if (quantizationType != QuantizationType.NoQuantization)
            fileName += $"_{quantizationType.ToString().ToLowerInvariant()}";
#pragma warning restore CA1308
        var modelPath = Path.Combine(storagePath, $"{fileName}.bin");
        using var fileWriter = File.OpenWrite(modelPath);
        await modelStream.CopyToAsync(fileWriter).ConfigureAwait(false);

        return modelPath;
    }
}