using System.Diagnostics.CodeAnalysis;

namespace LangChain.Providers.Automatic1111;

/// <summary>
/// 
/// </summary>
public class Automatic1111ModelSettings : TextToImageSettings
{
    public new static Automatic1111ModelSettings Default { get; } = new()
    {
        NegativePrompt = string.Empty,
        Seed = -1,
        Steps = 20,
        CfgScale = 6.0F,
        Width = 512,
        Height = 512,
        Sampler = "Euler a",
    };

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(NegativePrompt))]
    public string? NegativePrompt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Seed))]
    public int? Seed { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Steps))]
    public int? Steps { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(CfgScale))]
    public float? CfgScale { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Width))]
    public int? Width { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Height))]
    public int? Height { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNull(nameof(Sampler))]
    public string? Sampler { get; set; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Automatic1111ModelSettings Calculate(
        TextToImageSettings? requestSettings,
        TextToImageSettings? modelSettings,
        TextToImageSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as Automatic1111ModelSettings;
        var modelSettingsCasted = modelSettings as Automatic1111ModelSettings;
        var providerSettingsCasted = providerSettings as Automatic1111ModelSettings;

        return new Automatic1111ModelSettings
        {
            NegativePrompt =
                requestSettingsCasted?.NegativePrompt ??
                modelSettingsCasted?.NegativePrompt ??
                providerSettingsCasted?.NegativePrompt ??
                Default.NegativePrompt ??
                throw new InvalidOperationException("Default NegativePrompt is not set."),
            Seed =
                requestSettingsCasted?.Seed ??
                modelSettingsCasted?.Seed ??
                providerSettingsCasted?.Seed ??
                Default.Seed ??
                throw new InvalidOperationException("Default Seed is not set."),
            Steps =
                requestSettingsCasted?.Steps ??
                modelSettingsCasted?.Steps ??
                providerSettingsCasted?.Steps ??
                Default.Steps ??
                throw new InvalidOperationException("Default Steps is not set."),
            CfgScale =
                requestSettingsCasted?.CfgScale ??
                modelSettingsCasted?.CfgScale ??
                providerSettingsCasted?.CfgScale ??
                Default.CfgScale ??
                throw new InvalidOperationException("Default CfgScale is not set."),
            Width =
                requestSettingsCasted?.Width ??
                modelSettingsCasted?.Width ??
                providerSettingsCasted?.Width ??
                Default.Width ??
                throw new InvalidOperationException("Default Width is not set."),
            Height =
                requestSettingsCasted?.Height ??
                modelSettingsCasted?.Height ??
                providerSettingsCasted?.Height ??
                Default.Height ??
                throw new InvalidOperationException("Default Height is not set."),
            Sampler =
                requestSettingsCasted?.Sampler ??
                modelSettingsCasted?.Sampler ??
                providerSettingsCasted?.Sampler ??
                Default.Sampler ??
                throw new InvalidOperationException("Default Sampler is not set."),
        };
    }
}
