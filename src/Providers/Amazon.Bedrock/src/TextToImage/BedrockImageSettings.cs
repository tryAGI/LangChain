// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;

public class BedrockImageSettings : TextToImageSettings
{
    public new static BedrockImageSettings Default { get; } = new()
    {
       Height = 1024, 
       Width = 1024,
       Seed = 0,
       NumOfImages = 1,
    };

    /// <summary>
    /// 
    /// </summary>
    public int? Height { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public int? Width { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public int? Seed { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public int? NumOfImages { get; init; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static BedrockImageSettings Calculate(
        TextToImageSettings? requestSettings,
        TextToImageSettings? modelSettings,
        TextToImageSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as BedrockImageSettings;
        var modelSettingsCasted = modelSettings as BedrockImageSettings;
        var providerSettingsCasted = providerSettings as BedrockImageSettings;

        return new BedrockImageSettings
        {
          Height =
              requestSettingsCasted?.Height ??
              modelSettingsCasted?.Height ??
              providerSettingsCasted?.Height ??
              Default.Height ??
              throw new InvalidOperationException("Default Height is not set."),
          Width = 
              requestSettingsCasted?.Width ??
              modelSettingsCasted?.Width ??
              providerSettingsCasted?.Width ??
              Default.Width ??
              throw new InvalidOperationException("Default Width is not set."),
          Seed =
              requestSettingsCasted?.Seed ??
              modelSettingsCasted?.Seed ??
              providerSettingsCasted?.Seed ??
              Default.Seed ??
              throw new InvalidOperationException("Default Seed is not set."),
          NumOfImages =
              requestSettingsCasted?.NumOfImages ??
              modelSettingsCasted?.NumOfImages ??
              providerSettingsCasted?.NumOfImages ??
              Default.NumOfImages ??
              throw new InvalidOperationException("Default NumOfImages is not set."),
        };
    }
}