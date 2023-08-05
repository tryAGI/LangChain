// using Azure;
// using Azure.AI.OpenAI;
// using LangChain.Schema;
// using Microsoft.DeepDev;
//
// namespace LangChain.LLMS.AzureOpenAi
// {
//     public class AzureOpenAi : BaseLlm, IDisposable
//     {
//         private readonly AzureOpenAiConfiguration _configuration;
//         private readonly OpenAIClient _client;
//
//         public AzureOpenAi() : this(new AzureOpenAiConfiguration())
//         {
//         }
//
//         public AzureOpenAi(AzureOpenAiConfiguration configuration, OpenAIClient? client = null) : base(configuration)
//         {
//             _configuration = configuration;
//
//             if (string.IsNullOrEmpty(_configuration.ApiKey))
//             {
//                 _configuration.ApiKey = Environment.GetEnvironmentVariable("AZURE_OPEN_AI_API_KEY") ?? throw new ArgumentException("'AZURE_OPEN_AI_API_KEY' environment variable is not set and an API key is not provided in the input parameters");
//             }
//             if (string.IsNullOrEmpty(_configuration.Endpoint))
//             {
//                 _configuration.ApiKey = Environment.GetEnvironmentVariable("AZURE_OPEN_AI_ENDPOINT") ?? throw new ArgumentException("'AZURE_OPEN_AI_ENDPOINT' environment variable is not set and an API key is not provided in the input parameters");
//             }
//
//             if(client == null) 
//             {
//                 _client = new OpenAIClient(new Uri(_configuration.Endpoint), new AzureKeyCredential(_configuration.ApiKey));
//             }
//             else
//             {
//                 _client = client;
//             }
//         }
//
//         public override string ModelType { get; set; }
//         public override string LlmType { get; set; }
//         public override TikTokenizer Tokenizer { get; set; }
//
//         public override async Task<LlmResult> Generate(string[] prompts, List<string>? stopSequences)
//         {
//             var choices = new List<Choice>();
//             var usage = new List<CompletionsUsage>();
//
//             foreach (var prompt in prompts)
//             {
//                 CompletionsOptions completionsOptions = CreateCompletionsOptions(stopSequences, prompt);
//
//                 Response<Completions> completionsResponse = await _client.GetCompletionsAsync(_configuration.ModelName, completionsOptions);
//
//                 choices.AddRange(completionsResponse.Value.Choices);
//                 usage.Add(completionsResponse.Value.Usage);
//
//             }
//
//             return new LlmResult
//             {
//                 Generations = choices.Select(choice => new Generation()
//                 {
//                     Text = choice.Text,
//                     GenerationInfo = new Dictionary<string, object>(1)
//                 {
//                     { "finish_reason", choice.FinishReason }
//                 }
//                 }).ToArray(),
//                 LlmOutput = new Dictionary<string, object>(1)
//             {
//                 {"usage", usage}
//             }
//             };
//         }
//
//         private CompletionsOptions CreateCompletionsOptions(List<string>? stopSequences, string prompt)
//         {
//             CompletionsOptions completionsOptions = new()
//             {
//                 Temperature = _configuration.Temperature,
//                 MaxTokens = _configuration.MaxTokens,
//                 LogProbabilityCount = _configuration.LogProbabilityCount,
//                 NucleusSamplingFactor = _configuration.NucleusSamplingFactor,
//                 PresencePenalty = _configuration.PresencePenalty
//             };
//
//             if (stopSequences != null)
//             {
//                 foreach (var stop in stopSequences)
//                 {
//                     completionsOptions.StopSequences.Add(stop);
//                 }
//             }
//
//             completionsOptions.Prompts.Add(prompt);
//             return completionsOptions;
//         }
//
//         public void Dispose()
//         {
//             GC.SuppressFinalize(this);
//         }
//     }
// }
