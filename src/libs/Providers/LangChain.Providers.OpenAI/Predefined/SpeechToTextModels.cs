using OpenAI.Constants;

namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="SpeechToTextModels.Whisper1" />
public class Whisper1Model(OpenAiProvider provider)
    : OpenAiSpeechToTextModel(provider, id: SpeechToTextModels.Whisper1);