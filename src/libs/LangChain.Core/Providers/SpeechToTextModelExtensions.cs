namespace LangChain.Providers;

/// <summary>
/// 
/// </summary>
public static class SpeechToTextModelExtensions
{
    /// <inheritdoc cref="ISpeechToTextModel.TranscribeAsync"/>
    public static Task<string> TranscribeAsync<TSettings>(
        this ISpeechToTextModel<TSettings> model,
        byte[] bytes,
        TSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        model = model ?? throw new ArgumentNullException(nameof(model));
        bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));

        using var stream = new MemoryStream(bytes);
        
        return model.TranscribeAsync(
            stream: stream,
            settings: settings,
            cancellationToken: cancellationToken);
    }
    
    /// <inheritdoc cref="ISpeechToTextModel.TranscribeAsync"/>
    public static Task<string> TranscribeAsync<TSettings>(
        this ISpeechToTextModel<TSettings> model,
        string path,
        TSettings? settings = default,
        CancellationToken cancellationToken = default)
    {
        model = model ?? throw new ArgumentNullException(nameof(model));
        path = path ?? throw new ArgumentNullException(nameof(path));

        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        
        return model.TranscribeAsync(
            stream: stream,
            settings: settings,
            cancellationToken: cancellationToken);
    }
}