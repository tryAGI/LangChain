namespace LangChain.Providers
{
    /// <summary>
    /// Configuration options for Azure OpenAI
    /// </summary>
    public class AzureOpenAIConfiguration
    {
        /// <summary>
        /// Context size
        /// How much tokens model will remember.
        /// Most models have 2048
        /// </summary>
        public int ContextSize { get; set; } = 2048;

        /// <summary>
        /// Temperature
        /// controls the apparent creativity of generated completions. 
        /// Has a valid range of 0.0 to 2.0
        /// Defaults to 1.0 if not otherwise specified.
        /// </summary>
        public float Temperature { get; set; } = 0.7f;

        /// <summary>
        /// Gets the maximum number of tokens to generate. Has minimum of 0.
        /// </summary>
        public int MaxTokens { get; set; } = 800;

        /// <summary>
        /// Number of choices that should be generated per provided prompt. 
        /// Has a valid range of 1 to 128.
        /// </summary>
        public int ChoiceCount { get; set; } = 1;

        /// <summary>
        /// Azure OpenAI API Key
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Deployment name
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Azure OpenAI Resource URI
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;
    }
}
