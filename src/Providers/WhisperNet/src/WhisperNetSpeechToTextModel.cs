using System.Text;
using Whisper.net;

namespace LangChain.Providers.WhisperNet;


public class WhisperNetSpeechToTextModel : Model<SpeechToTextSettings>, ISpeechToTextModel
{

    public WhisperNetSpeechToTextConfiguration Configuration { get; }

    [CLSCompliant(false)]
    public WhisperFactory Factory { get; }

    /// <summary>
    /// Wrapper around Whisper.Net models
    /// </summary>
    /// <param name="id"></param>
    /// <param name="configuration"></param>
    public WhisperNetSpeechToTextModel(string id, WhisperNetSpeechToTextConfiguration configuration) : base(id)
    {
        Configuration = configuration;
        Factory = WhisperFactory.FromPath(Configuration.PathToModelFile);
    }

    /// <summary>
    /// Loads openai whisper model from path
    /// </summary>
    /// <param name="path"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static WhisperNetSpeechToTextModel FromPath(string path, WhisperNetSpeechToTextConfiguration? configuration = null)
    {
        configuration ??= new WhisperNetSpeechToTextConfiguration();
        configuration.PathToModelFile = path;
        return new WhisperNetSpeechToTextModel(Path.GetFileNameWithoutExtension(path), configuration);
    }

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public async Task<SpeechToTextResponse> TranscribeAsync(SpeechToTextRequest request, SpeechToTextSettings? settings = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        request = request ?? throw new ArgumentNullException(nameof(request));
        try
        {
            var builder = Factory.CreateBuilder();
            if (Configuration.Temperature.HasValue) builder.WithTemperature(Configuration.Temperature.Value);
            if (!string.IsNullOrEmpty(Configuration.Language)) builder.WithLanguage(Configuration.Language!);
            if (!string.IsNullOrEmpty(Configuration.Prompt)) builder.WithPrompt(Configuration.Prompt!);

            using var audioProcessor = builder.Build();
            var sb = new StringBuilder();
            await foreach (var result in audioProcessor.ProcessAsync(request.Stream, cancellationToken))
            {
                sb.Append(result.Text);
            }

            return new SpeechToTextResponse()
            {
                Text = sb.ToString(),
                Usage = Usage
            };
        }
        finally
        {
            if (request.OwnsStream)
            {
#if NET6_0_OR_GREATER
                await request.Stream.DisposeAsync().ConfigureAwait(false);
#else
                request.Stream.Dispose();
#endif
            }
        }
    }
}