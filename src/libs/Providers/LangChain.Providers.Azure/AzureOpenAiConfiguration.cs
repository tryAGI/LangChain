// using LangChain.Cache;
//
// namespace LangChain.LLMS.AzureOpenAi
// {
//     public class AzureOpenAiConfiguration : IBaseLlmParams
//     {
//         public AzureOpenAiConfiguration() 
//         {
//             ModelName = "text-davinci-003";
//         }
//         /// <summary>
//         ///     Gets or sets the sampling temperature to use that controls the apparent creativity of generated
//         ///     completions.
//         ///     Has a valid range of 0.0 to 2.0 and defaults to 1.0 if not otherwise specified.
//         /// </summary>
//         /// <remarks>
//         ///     Higher values will make output more random while lower values will make results more focused and
//         ///     deterministic.
//         ///
//         ///     It is not recommended to modify <see cref="Temperature"/> and <see cref="NucleusSamplingFactor"/>
//         ///     for the same completions request as the interaction of these two settings is difficult to predict.
//         /// </remarks>
//         public float? Temperature { get; set; }
//
//         /// <summary> Gets the maximum number of tokens to generate. Has minimum of 0. </summary>
//         /// <remarks>
//         ///     <see cref="MaxTokens"/> is equivalent to 'max_tokens' in the REST request schema.
//         /// </remarks>
//         public int? MaxTokens { get; set; }
//
//         /// <summary>
//         ///     Gets or sets a value that influences the probability of generated tokens appearing based on their
//         ///     cumulative frequency in generated text.
//         ///     Has a valid range of -2.0 to 2.0.
//         /// </summary>
//         /// <remarks>
//         ///     Positive values will make tokens less likely to appear as their frequency increases and decrease the
//         ///     model's likelihood of repeating the same statements verbatim.
//         /// </remarks>
//         public float? FrequencyPenalty { get; set; }
//
//         /// <summary>
//         ///     Gets or sets a value that controls generation of log probabilities on the
//         ///     <see cref="LogProbabilityCount"/> most likely tokens.
//         ///     Has a valid range of 0 to 5.
//         /// </summary>
//         /// <remarks>
//         ///     <see cref="LogProbabilityCount"/> is equivalent to 'logprobs' in the REST request schema.
//         /// </remarks>
//         public int? LogProbabilityCount { get; set; }
//
//         /// <summary>
//         ///     Gets or set a an alternative value to <see cref="Temperature"/>, called nucleus sampling, that causes
//         ///     the model to consider the results of the tokens with <see cref="NucleusSamplingFactor"/> probability
//         ///     mass.
//         /// </summary>
//         /// <remarks>
//         ///     As an example, a value of 0.1 will cause only the tokens comprising the top 10% of probability mass to
//         ///     be considered.
//         ///
//         ///     It is not recommended to modify <see cref="Temperature"/> and <see cref="NucleusSamplingFactor"/>
//         ///     for the same completions request as the interaction of these two settings is difficult to predict.
//         ///
//         ///     <see cref="NucleusSamplingFactor"/> is equivalent to 'top_p' in the REST request schema.
//         /// </remarks>
//         public float? NucleusSamplingFactor { get; set; }
//
//         /// <summary>
//         ///     Gets or sets a value that influences the probability of generated tokens appearing based on their
//         ///     existing presence in generated text.
//         ///     Has a valid range of -2.0 to 2.0.
//         /// </summary>
//         /// <remarks>
//         ///     Positive values will make tokens less likely to appear when they already exist and increase the
//         ///     model's likelihood to output new topics.
//         /// </remarks>
//         public float? PresencePenalty { get; set; }
//
//         public string Endpoint { get; set; }
//         /// <summary>
//         /// OpenAI API Key.
//         /// </summary>
//         public string ApiKey { get; set; }
//         public decimal? Concurrency { get; set; }
//         public BaseCache? Cache { get; set; }
//         public bool? Verbose { get; set; }
//         /// <summary>
//         /// Model name to use
//         /// </summary>
//         public string ModelName { get; set; }
//     }
// }
