using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;

// ReSharper disable once CheckNamespace
namespace LangChain.Providers.Amazon.Bedrock;


public class AmazonKnowledgeBaseChatSettings : BedrockChatSettings
{
    public new static AmazonKnowledgeBaseChatSettings Default { get; } = new()
    {
        SelectedSearchType = "VECTOR",
        KnowledgeBaseId = null
    };

    /// <summary>
    /// Knowledge base id
    /// </summary>
    public required string? KnowledgeBaseId { get; init; }

    /// <summary>
    /// Knowledge base search type
    /// </summary>
    public SearchType? SelectedSearchType { get; init; }

    /// <summary>
    /// Knowledge base filter
    /// </summary>
    public RetrievalFilter? Filter { get; set; }

    /// <summary>
    /// Knowledge base response citations
    /// </summary>
    public IReadOnlyList<Citation>? Citations { get; set; }

    /// <summary>
    /// Calculate the settings to use for the request.
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <param name="modelSettings"></param>
    /// <param name="providerSettings"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public new static AmazonKnowledgeBaseChatSettings Calculate(
        ChatSettings? requestSettings,
        ChatSettings? modelSettings,
        ChatSettings? providerSettings)
    {
        var requestSettingsCasted = requestSettings as AmazonKnowledgeBaseChatSettings;
        var modelSettingsCasted = modelSettings as AmazonKnowledgeBaseChatSettings;
        var providerSettingsCasted = providerSettings as AmazonKnowledgeBaseChatSettings;

        return new AmazonKnowledgeBaseChatSettings
        {
            KnowledgeBaseId =
                requestSettingsCasted?.KnowledgeBaseId ??
                modelSettingsCasted?.KnowledgeBaseId ??
                providerSettingsCasted?.KnowledgeBaseId ??
                Default.KnowledgeBaseId ??
                throw new InvalidOperationException("KnowledgeBaseId can not be null."),

            SelectedSearchType =
                requestSettingsCasted?.SelectedSearchType ??
                modelSettingsCasted?.SelectedSearchType ??
                providerSettingsCasted?.SelectedSearchType ??
                Default.SelectedSearchType ??
                throw new InvalidOperationException("Default SelectedSearchType is not set."),

            Filter =
                requestSettingsCasted?.Filter ??
                modelSettingsCasted?.Filter ??
                providerSettingsCasted?.Filter ??
                Default.Filter ??
                throw new InvalidOperationException("Default Filter is not set."),
        };
    }
}