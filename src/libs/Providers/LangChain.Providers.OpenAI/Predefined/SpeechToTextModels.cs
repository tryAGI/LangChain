using OpenAI.Constants;

namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="SpeechToTextModels.Whisper1" />
public class Whisper1Model(string apiKey)
    : OpenAiSpeechToTextModel(new OpenAiProvider(apiKey), id: SpeechToTextModels.Whisper1);