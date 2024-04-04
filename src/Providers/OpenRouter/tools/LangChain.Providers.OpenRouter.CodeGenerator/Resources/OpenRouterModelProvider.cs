using OpenAI.Constants;

namespace LangChain.Providers.OpenRouter
{
    /// <summary>
    /// Contains all the OpenRouter models.
    /// </summary>
    public static class OpenRouterModelProvider
    {
        private static IReadOnlyDictionary<OpenRouterModelIds, ChatModels> Models { get; set; }
        static OpenRouterModelProvider()
        {
            var dic = new Dictionary<OpenRouterModelIds, ChatModels>
            {
                {{DicAdd}}
            };


            Models = dic;
        }

        public static ChatModels GetModelById(OpenRouterModelIds modelId)
        {
            if (Models.ContainsKey(modelId))
            {
                return Models[modelId];
            }
            else
            {
                throw new ArgumentException($"Invalid Open Router Model {modelId.ToString()}");
            }
        }
    }
}
