namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="CreateSpeechRequestModel.Tts1" />
public class Tts1Model(OpenAiProvider provider)
    : OpenAiTextToSpeechModel(provider, id: CreateSpeechRequestModel.Tts1);

/// <inheritdoc cref="CreateSpeechRequestModel.Tts1Hd" />
public class Tts1HdModel(OpenAiProvider provider)
    : OpenAiTextToSpeechModel(provider, id: CreateSpeechRequestModel.Tts1Hd);