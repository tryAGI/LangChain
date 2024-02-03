using OpenAI.Constants;

namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="TextToSpeechModels.Tts1" />
public class Tts1Model(string apiKey)
    : OpenAiTextToSpeechModel(new OpenAiProvider(apiKey), id: TextToSpeechModels.Tts1);

/// <inheritdoc cref="TextToSpeechModels.Tts1Hd" />
public class Tts1HdModel(string apiKey)
    : OpenAiTextToSpeechModel(new OpenAiProvider(apiKey), id: TextToSpeechModels.Tts1Hd);