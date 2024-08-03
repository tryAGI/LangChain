namespace LangChain.Providers.OpenAI.Predefined;

/// <inheritdoc cref="CreateTranscriptionRequestModel.Whisper1" />
public class Whisper1Model(OpenAiProvider provider)
    : OpenAiSpeechToTextModel(provider, id: CreateTranscriptionRequestModel.Whisper1);