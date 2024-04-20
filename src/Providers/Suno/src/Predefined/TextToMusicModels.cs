namespace LangChain.Providers.Suno.Predefined;

/// <summary>
/// 
/// </summary>
/// <param name="provider"></param>
public class ChirpV3Model(SunoProvider provider)
    : SunoModel(provider, id: "chirp-v3-0");

/// <summary>
/// 
/// </summary>
/// <param name="provider"></param>
public class ChirpV2Model(SunoProvider provider)
    : SunoModel(provider, id: "chirp-v2-xxl-alpha");