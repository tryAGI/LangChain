// ReSharper disable once CheckNamespace
namespace LangChain.Providers.OpenAI;

public class OpenAiChatSettings : ChatSettings
{
    /// <summary>
    /// According: https://platform.openai.com/docs/api-reference/chat/create
    /// </summary>
    public new static OpenAiChatSettings Default { get; } = new()
    {
        StopSequences = ChatSettings.Default.StopSequences,
        User = ChatSettings.Default.User,
        UseStreaming = ChatSettings.Default.UseStreaming,
        Temperature = null,
        MaxTokens = null,
        TopP = null,
        FrequencyPenalty = null,
        PresencePenalty = null,
        Number = null,
        LogitBias = null,
    };

    /// <summary>
    /// What sampling temperature to use, between 0 and 2. <br/>
    /// Higher values like 0.8 will make the output more random,
    /// while lower values like 0.2 will make it more focused and deterministic. <br/>
    /// We generally recommend altering this or <see cref="TopP"/> but not both. <br/>
    /// Defaults to 1. <br/>
    /// </summary>
    public double? Temperature { get; init; }
    
    /// <summary>
    /// The maximum number of tokens to generate in the chat completion. <br/>
    /// The total length of input tokens and generated tokens is limited by the model's context length. <br/>
    /// Defaults to int.MaxValue. <br/>
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// An alternative to sampling with temperature, called nucleus sampling,
    /// where the model considers the results of the tokens with top_p probability mass.
    /// So 0.1 means only the tokens comprising the top 10% probability mass are considered. <br/>
    /// We generally recommend altering this or <see cref="Temperature"/> but not both. <br/>
    /// Defaults to 1. <br/>
    /// </summary>
    public double? TopP { get; set; }

    /// <summary>
    /// Number between -2.0 and 2.0. <br/>
    /// Positive values penalize new tokens based on their existing frequency in the text so far,
    /// decreasing the model's likelihood to repeat the same line verbatim. <br/>
    /// Defaults to 0. <br/>
    /// </summary>
    public double? FrequencyPenalty { get; set; }

    /// <summary>
    /// Number between -2.0 and 2.0. <br/>
    /// Positive values penalize new tokens based on whether they appear in the text so far,
    /// increasing the model's likelihood to talk about new topics. <br/>
    /// Defaults to 0. <br/>
    /// </summary>
    public double? PresencePenalty { get; set; }

    /// <summary>
    /// How many chat completion choices to generate for each input message. <br/>
    /// Defaults to 1. <br/>
    /// </summary>
    public int? Number { get; set; }

    /// <summary>
    /// Modify the likelihood of specified tokens appearing in the completion. <br/>
    /// Accepts a json object that maps tokens (specified by their token ID in the tokenizer)
    /// to an associated bias value from -100 to 100.
    /// Mathematically, the bias is added to the logits generated by the model prior to sampling. <br/>
    /// The exact effect will vary per model, but values between -1 and 1 should decrease or increase likelihood of selection;
    /// values like -100 or 100 should result in a ban or exclusive selection of the relevant token. <br/>
    /// </summary>
    public IReadOnlyDictionary<string, double>? LogitBias { get; set; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static OpenAiChatSettings Calculate(
        ChatSettings? requestSettings,
        ChatSettings? modelSettings,
        ChatSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as OpenAiChatSettings;
        var modelSettingsCasted = modelSettings as OpenAiChatSettings;
        var providerSettingsCasted = providerSettings as OpenAiChatSettings;
        
        return new OpenAiChatSettings
        {
            StopSequences = 
                requestSettings?.StopSequences ??
                modelSettings?.StopSequences ??
                providerSettings?.StopSequences ??
                Default.StopSequences ??
                throw new InvalidOperationException("Default StopSequences is not set."),
            User = 
                requestSettings?.User ??
                modelSettings?.User ??
                providerSettings?.User ??
                Default.User ??
                throw new InvalidOperationException("Default User is not set."),
            UseStreaming = 
                requestSettings?.UseStreaming ??
                modelSettings?.UseStreaming ??
                providerSettings?.UseStreaming ??
                Default.UseStreaming ??
                throw new InvalidOperationException("Default UseStreaming is not set."),
            Temperature = 
                requestSettingsCasted?.Temperature ??
                modelSettingsCasted?.Temperature ??
                providerSettingsCasted?.Temperature ??
                Default.Temperature,
            MaxTokens =
                requestSettingsCasted?.MaxTokens ??
                modelSettingsCasted?.MaxTokens ??
                providerSettingsCasted?.MaxTokens ??
                Default.MaxTokens,
            TopP =
                requestSettingsCasted?.TopP ??
                modelSettingsCasted?.TopP ??
                providerSettingsCasted?.TopP ??
                Default.TopP,
            FrequencyPenalty =
                requestSettingsCasted?.FrequencyPenalty ??
                modelSettingsCasted?.FrequencyPenalty ??
                providerSettingsCasted?.FrequencyPenalty ??
                Default.FrequencyPenalty,
            PresencePenalty =
                requestSettingsCasted?.PresencePenalty ??
                modelSettingsCasted?.PresencePenalty ??
                providerSettingsCasted?.PresencePenalty ??
                Default.PresencePenalty,
            Number =
                requestSettingsCasted?.Number ??
                modelSettingsCasted?.Number ??
                providerSettingsCasted?.Number ??
                Default.Number,
            LogitBias =
                requestSettingsCasted?.LogitBias ??
                modelSettingsCasted?.LogitBias ??
                providerSettingsCasted?.LogitBias ??
                Default.LogitBias,
        };
    }
}