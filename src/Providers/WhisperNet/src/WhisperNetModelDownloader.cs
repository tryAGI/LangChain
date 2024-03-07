using Whisper.net.Ggml;

namespace LangChain.Providers.WhisperNet;

[CLSCompliant(false)]
public sealed class WhisperNetModelDownloader
{
    /// <summary>
    /// The default storage path for the models
    /// </summary>
    /// <returns>Model path</returns>
    public static string DefaultStoragePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LangChain", "CSharp", "Models", "WhisperNet");
    
    public static async Task<string> DownloadModel(GgmlType type, QuantizationType quantizationType, bool usingCoreMl = false, string? storagePath = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        storagePath ??= DefaultStoragePath;
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
        }

        if (usingCoreMl)
        {
            await WhisperGgmlDownloader.GetEncoderCoreMLModelAsync(type, cancellationToken)
                .ExtractToPath(storagePath)
                .ConfigureAwait(false);
        }
        
        using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(type, quantizationType, cancellationToken).ConfigureAwait(false);
        
#pragma warning disable CA1308
        var fileName = $"gglm-{type.ToString().ToLowerInvariant()}";
        if (quantizationType != QuantizationType.NoQuantization)
            fileName += $"-{quantizationType.ToString().ToLowerInvariant()}";
#pragma warning restore CA1308
        var modelPath = Path.Combine(storagePath, $"{fileName}.bin");
        using var fileWriter = File.OpenWrite(modelPath);
        await modelStream.CopyToAsync(fileWriter, cancellationToken).ConfigureAwait(false);

        return modelPath;
    }
}