using OpenAI.Constants;

namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="TextToSpeechModels.Tts1" />
public class Tts1Model(OpenAiProvider provider)
    : OpenAiTextToSpeechModel(provider, id: TextToSpeechModels.Tts1);

/// <inheritdoc cref="TextToSpeechModels.Tts1Hd" />
public class Tts1HdModel(OpenAiProvider provider)
    : OpenAiTextToSpeechModel(provider, id: TextToSpeechModels.Tts1Hd);